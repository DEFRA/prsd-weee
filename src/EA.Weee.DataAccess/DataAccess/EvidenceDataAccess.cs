namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using CuttingEdge.Conditions;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Domain.Evidence;
    using Prsd.Core.Domain;
    using Z.EntityFramework.Plus;

    public class EvidenceDataAccess : IEvidenceDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;

        public EvidenceDataAccess(WeeeContext context, 
            IUserContext userContext, IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.userContext = userContext;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<Note> GetNoteById(Guid id)
        {
            var note = await context.Notes
                .Include(n => n.NoteTonnage)
                .Include(n => n.NoteTransferTonnage)
                .Include(nt => nt.NoteTransferTonnage.Select(nt1 => nt1.NoteTonnage))
                .Include(nt => nt.NoteTransferTonnage.Select(nt1 => nt1.NoteTonnage.Note))
                .Include(n => n.NoteStatusHistory)
                .FirstOrDefaultAsync(n => n.Id == id);

            Condition.Requires(note).IsNotNull($"Evidence note {id} not found");

            return note;
        }

        public async Task<Note> Update(Note note, Scheme recipient, DateTime startDate, DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            IList<NoteTonnage> tonnages,
            NoteStatus status,
            DateTime updateDate)
        {
            note.Update(recipient, startDate, endDate, wasteType, protocol);

            if (status.Equals(NoteStatus.Submitted))
            {
                note.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), updateDate);
            }

            foreach (var noteTonnage in tonnages)
            {
                var tonnage = note.NoteTonnage.First(t => t.CategoryId.Equals(noteTonnage.CategoryId));

                tonnage.UpdateValues(noteTonnage.Received, noteTonnage.Reused);
            }

            await context.SaveChangesAsync();

            return note;
        }
        public async Task<List<Note>> GetAllNotes(NoteFilter filter)
        {
            var allowedStatus = filter.AllowedStatuses.Select(v => v.Value);
            var allowedNoteTypes = filter.NoteTypeFilter.Select(n => n.Value);

            var submittedStartDateFilter = filter.StartDateSubmitted?.Date;
            var submittedEndDateFilter = filter.EndDateSubmitted?.Date;

            Guid? groupedAatfId = null;
            if (filter.AatfId.HasValue)
            {
                var aatf = await context.Aatfs.FindAsync(filter.AatfId);

                Condition.Requires(aatf).IsNotNull();

                groupedAatfId = aatf.AatfId;
            }

            var notes = context.Notes
                .Include(n => n.Organisation.Schemes)
               .Where(p => p.ComplianceYear.Equals(filter.ComplianceYear)
                            && allowedNoteTypes.Contains(p.NoteType.Value)
                            && ((!filter.OrganisationId.HasValue || p.Organisation.Id == filter.OrganisationId.Value)
                                && (!filter.SchemeId.HasValue || p.Recipient.Id == filter.SchemeId)
                                && (filter.NoteStatusId.HasValue && p.Status.Value == filter.NoteStatusId
                                    || !filter.NoteStatusId.HasValue && allowedStatus.Contains(p.Status.Value)))
                            && (!filter.StartDateSubmitted.HasValue
                                || p.NoteStatusHistory.Any(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                                && DbFunctions.TruncateTime(p.NoteStatusHistory.Where(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                                    .OrderByDescending(nsh1 => nsh1.ChangedDate).FirstOrDefault().ChangedDate) >= submittedStartDateFilter)
                            && (!filter.EndDateSubmitted.HasValue
                                || p.NoteStatusHistory.Any(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                                && DbFunctions.TruncateTime(p.NoteStatusHistory.Where(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                                    .OrderByDescending(nsh1 => nsh1.ChangedDate).FirstOrDefault().ChangedDate) <= submittedEndDateFilter)
                            && (!filter.WasteTypeId.HasValue || (int)p.WasteType == filter.WasteTypeId)
                            && (filter.SearchRef == null ||
                                (filter.FormattedNoteType > 0 ?
                                    (filter.FormattedNoteType == p.NoteType.Value && filter.FormattedSearchRef == p.Reference.ToString()) :
                                    (filter.FormattedSearchRef == p.Reference.ToString()))));
            
            if (groupedAatfId.HasValue)
            {
                return await notes.Where(p => p.Aatf.AatfId == groupedAatfId).ToListAsync();
            }
            
            return await notes.ToListAsync();
        }

        public async Task<int> GetComplianceYearByNotes(List<Guid> evidenceNoteIds)
        {
            var note = await context.Notes.Where(n => evidenceNoteIds.Contains(n.Id))
                .OrderBy(n1 => n1.StartDate).FirstAsync();

            return note.ComplianceYear;
        }

        public async Task<IEnumerable<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes, int complianceYear)
        {
            var notes = await context.Notes
                .IncludeFilter(n => n.NoteTonnage.Where(nt => nt.Received.HasValue && categories.Contains((int)nt.CategoryId)))
                .IncludeFilter(n => n.NoteTonnage.Select(nt => nt.NoteTransferTonnage))
                .Where(n => n.RecipientId == schemeId &&
                    n.NoteType.Value == NoteType.EvidenceNote.Value &&
                    n.WasteType.Value == WasteType.HouseHold &&
                    n.Status.Value == NoteStatus.Approved.Value &&
                    n.ComplianceYear == complianceYear &&
                    (evidenceNotes.Count == 0 || evidenceNotes.Contains(n.Id)) &&
                    n.NoteTonnage.Where(nt => nt.Received != null)
                        .Select(nt1 => (int)nt1.CategoryId).AsEnumerable().Any(e => categories.Contains(e))).ToListAsync();

            return notes;
        }

        public async Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId, int complianceYear)
        {
            return await context.Notes.Where(n => (n.AatfId.HasValue && n.AatfId.Value.Equals(aatfId)) && n.Status.Value.Equals(status.Value) && n.ComplianceYear.Equals(complianceYear))
                .CountAsync();
        }

        public async Task<Guid> AddTransferNote(Organisation organisation, 
            Scheme scheme,
            List<NoteTransferTonnage> transferTonnage, 
            NoteStatus status, 
            int complianceYear,
            string userId,
            DateTime date)
        {
            var evidenceNote = new Note(organisation,
                scheme,
                userId,
                transferTonnage,
                complianceYear,
                WasteType.HouseHold);

            if (status.Equals(NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), date);
            }

            var note = await genericDataAccess.Add(evidenceNote);

            await context.SaveChangesAsync();

            return note.Id;
        }

        public async Task<List<NoteTonnage>> GetTonnageByIds(List<Guid> ids)
        {
            return await context.NoteTonnages
                .Include(n => n.Note)
                .Include(n => n.NoteTransferTonnage)
                .Include(n => n.NoteTransferTonnage.Select(nt => nt.TransferNote))
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();
        }
    }
}

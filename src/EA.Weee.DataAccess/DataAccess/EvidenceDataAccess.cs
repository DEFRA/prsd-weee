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
                .Include(n => n.NoteTransferCategories)
                .Include(n => n.NoteStatusHistory)
                .FirstOrDefaultAsync(n => n.Id == id);

            Condition.Requires(note).IsNotNull($"Evidence note {id} not found");

            return note;
        }

        public async Task<Note> Update(Note note, Scheme recipient, DateTime startDate, DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            IList<NoteTonnage> tonnages,
            NoteStatus status)
        {
            note.Update(recipient, startDate, endDate, wasteType, protocol);

            if (status.Equals(NoteStatus.Submitted))
            {
                note.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString());
            }

            foreach (var noteTonnage in tonnages)
            {
                var tonnage = note.NoteTonnage.First(t => t.CategoryId.Equals(noteTonnage.CategoryId));

                tonnage.UpdateValues(noteTonnage.Received, noteTonnage.Reused);
            }

            await context.SaveChangesAsync();

            return note;
        }
        public async Task<List<Note>> GetAllNotes(EvidenceNoteFilter filter)
        {
            var allowedStatus = filter.AllowedStatuses.Select(v => v.Value);

            var submittedStartDateFilter = filter.StartDateSubmitted?.Date;
            var submittedEndDateFilter = filter.EndDateSubmitted?.Date;

            var notes = await context.Notes
               .Where(p => p.ComplianceYear.Equals((short)filter.ComplianceYear) && 
                           p.NoteType.Value == NoteType.EvidenceNote.Value &&
                           ((!filter.OrganisationId.HasValue || p.Organisation.Id == filter.OrganisationId.Value)
                            && (!filter.AatfId.HasValue || p.Aatf.Id == filter.AatfId.Value)
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
                                    (filter.FormattedSearchRef == p.Reference.ToString()))))
               .ToListAsync();

            return notes;
        }

        public async Task<int> GetComplianceYearByNotes(List<Guid> evidenceNoteIds)
        {
            var note = await context.Notes.Where(n => evidenceNoteIds.Contains(n.Id))
                .OrderBy(n1 => n1.StartDate).FirstAsync();

            return note.ComplianceYear;
        }

        public async Task<IEnumerable<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes)
        {
            var notes = await context.Notes
                .IncludeFilter(n => n.NoteTonnage.Where(nt => 
                    nt.Received.HasValue && categories.Contains((int)nt.CategoryId)))
                .IncludeFilter(n => n.NoteTonnage.Select(nt => nt.NoteTransferTonnage))
                .IncludeFilter(n => n.NoteTonnage.Select(nt => nt.NoteTransferTonnage).Select(ntt => ntt.Select(nttt => nttt.TransferNote)))
                .Where(n => n.RecipientId == schemeId &&
                    n.NoteType.Value == NoteType.EvidenceNote.Value &&
                    n.Status.Value == NoteStatus.Approved.Value &&
                    n.NoteTonnage.Where(nt => nt.Received != null).Select(nt1 => (int)nt1.CategoryId).AsEnumerable().Any(e => categories.Contains(e)) && evidenceNotes.Count == 0 || evidenceNotes.Contains(n.Id)).ToListAsync();

            return notes;
        }
        public async Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId)
        {
            return await context.Notes.Where(n => (n.AatfId.HasValue && n.AatfId.Value.Equals(aatfId)) && n.Status.Value.Equals(status.Value))
                .CountAsync();
        }

        public async Task<Guid> AddTransferNote(Organisation organisation, 
            Scheme scheme, 
            List<NoteTransferCategory> transferCategories,
            List<NoteTransferTonnage> transferTonnage, 
            NoteStatus status, 
            int complianceYear,
            string userId)
        {
            var evidenceNote = new Note(organisation,
                scheme,
                userId,
                transferTonnage,
                transferCategories,
                complianceYear);

            if (status.Equals(NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString());
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

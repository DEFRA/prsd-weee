﻿namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using CuttingEdge.Conditions;
    using Domain.Organisation;
    using EA.Weee.Domain.Evidence;
    using Prsd.Core.Domain;

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
                .Include(nt => nt.NoteTransferTonnage.Select(nt1 => nt1.NoteTonnage.Note))
                .Include(n => n.NoteStatusHistory)
                .FirstOrDefaultAsync(n => n.Id == id);

            Condition.Requires(note).IsNotNull($"Evidence note {id} not found");

            return note;
        }

        public async Task<Note> Update(Note note, Organisation recipient, DateTime startDate, DateTime endDate,
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
        public async Task<EvidenceNoteResults> GetAllNotes(NoteFilter filter)
        {
            var allowedStatus = filter.AllowedStatuses.Select(v => v.Value).ToList();
            var allowedNoteTypes = filter.NoteTypeFilter.Select(n => n.Value).ToList();
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
                .Where(n => n.ComplianceYear == filter.ComplianceYear);

            if (allowedNoteTypes.Any())
            {
                notes = notes.Where(n => allowedNoteTypes.Contains(n.NoteType.Value));
            }
            if (filter.OrganisationId.HasValue)
            {
                notes = notes.Where(n => n.Organisation.Id == filter.OrganisationId.Value);
            }
            if (filter.RecipientId.HasValue)
            {
                notes = notes.Where(n => n.RecipientId == filter.RecipientId);
            }
            if (filter.NoteStatusId.HasValue)
            {
                notes = notes.Where(n => n.Status.Value == filter.NoteStatusId);
            }
            if (filter.AllowedStatuses.Any() && !filter.NoteStatusId.HasValue)
            {
                notes = notes.Where(n => allowedStatus.Contains(n.Status.Value));
            }
            if (submittedStartDateFilter.HasValue)
            {
                notes = notes.Where(n =>
                    n.NoteStatusHistory.Any(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                    && DbFunctions.TruncateTime(n.NoteStatusHistory
                        .Where(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value && nsh.NoteId == n.Id)
                        .OrderByDescending(nsh1 => nsh1.ChangedDate).FirstOrDefault().ChangedDate) >=
                    submittedStartDateFilter);
            }
            if (submittedEndDateFilter.HasValue)
            {
                notes = notes.Where(n =>
                    n.NoteStatusHistory.Any(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value)
                    && DbFunctions.TruncateTime(n.NoteStatusHistory
                        .Where(nsh => nsh.ToStatus.Value == NoteStatus.Submitted.Value && nsh.NoteId == n.Id)
                        .OrderByDescending(nsh1 => nsh1.ChangedDate).FirstOrDefault().ChangedDate) <=
                    submittedEndDateFilter);
            }
            if (filter.WasteTypeId.HasValue)
            {
                notes = notes.Where(n => (int)n.WasteType == filter.WasteTypeId);
            }
            if (filter.SearchRef != null)
            {
                notes = notes.Where(n => filter.FormattedNoteType > 0 ?
                                        (filter.FormattedNoteType == n.NoteType.Value && filter.FormattedSearchRef == n.Reference.ToString()) :
                                        (filter.FormattedSearchRef == n.Reference.ToString()));
            }
            if (groupedAatfId.HasValue)
            {
                notes = notes.Where(p => p.Aatf.AatfId == groupedAatfId);
            }

            var returnNotes = await notes.OrderByDescending(n => n.Reference)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new EvidenceNoteResults(returnNotes.ToList(), notes.Count());
        }

        public async Task<IEnumerable<int>> GetComplianceYearsForNotes(List<int> allowedStatuses)
        {
            var notes = context.Notes.Where(n => allowedStatuses.Contains(n.Status.Value));

            var complianceYearsList = await notes.Select(x => x.ComplianceYear).Distinct().OrderByDescending(y => y).ToListAsync();

            return complianceYearsList;
        }

        public async Task<int> GetComplianceYearByNotes(List<Guid> evidenceNoteIds)
        {
            var note = await context.Notes.Where(n => evidenceNoteIds.Contains(n.Id))
                .OrderBy(n1 => n1.StartDate).FirstAsync();

            return note.ComplianceYear;
        }

        public async Task<EvidenceNoteResults> GetNotesToTransfer(Guid recipientOrganisationId, 
            List<int> categories, 
            List<Guid> evidenceNotes, 
            int complianceYear,
            int pageNumber,
            int pageSize)
        {
            var notes = context.Notes
                .Include(n => n.NoteTonnage.Select(nt => nt.NoteTransferTonnage.Select(ntt => ntt.TransferNote)))
                .Where(n => n.RecipientId == recipientOrganisationId &&
                            n.NoteType.Value == NoteType.EvidenceNote.Value &&
                            n.WasteType.Value == WasteType.HouseHold &&
                            n.Status.Value == NoteStatus.Approved.Value &&
                            n.ComplianceYear == complianceYear &&
                            (evidenceNotes.Count == 0 || evidenceNotes.Contains(n.Id)) &&
                            n.NoteTonnage.Where(nt => nt.Received != null)
                                .Select(nt1 => (int)nt1.CategoryId).AsEnumerable().Any(e => categories.Contains(e)));

            var pagedNotes = await notes.OrderByDescending(n => n.Reference)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new EvidenceNoteResults(pagedNotes, notes.Count());
        }

        public async Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId, int complianceYear)
        {
            var aatf = await context.Aatfs.FindAsync(aatfId);
            var groupedAatfId = aatf.AatfId;

            return await context.Notes.Where(n => (n.AatfId.HasValue && n.Aatf.AatfId == groupedAatfId) && n.Status.Value.Equals(status.Value) && n.ComplianceYear.Equals(complianceYear))
                .CountAsync();
        }

        public async Task<Note> AddTransferNote(Organisation organisation, 
            Organisation recipientOrganisation,
            List<NoteTransferTonnage> transferTonnage, 
            NoteStatus status, 
            int complianceYear,
            string userId,
            DateTime date)
        {
            var evidenceNote = new Note(organisation,
                recipientOrganisation,
                userId,
                transferTonnage,
                complianceYear);

            if (status.Equals(NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), date);
            }

            var note = await genericDataAccess.Add(evidenceNote);

            await context.SaveChangesAsync();

            return note;
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

        public async Task<Note> UpdateTransfer(Note note, Organisation recipient,
            IList<NoteTransferTonnage> tonnages,
            NoteStatus status,
            DateTime updateDate)
        {
            if (status.Equals(NoteStatus.Submitted))
            {
                note.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), updateDate);
            }

            foreach (var noteTonnage in tonnages)
            {
                var existingTonnage = note.NoteTransferTonnage.FirstOrDefault(nt => nt.NoteTonnageId == noteTonnage.NoteTonnageId);

                if (existingTonnage == null)
                {
                    note.NoteTransferTonnage.Add(noteTonnage);
                }
                else
                {
                    existingTonnage.UpdateValues(noteTonnage.Received, noteTonnage.Reused);
                }
            }

            var itemsToRemove =
                note.NoteTransferTonnage.Where(ntt => tonnages.All(t => t.NoteTonnageId != ntt.NoteTonnageId));

            genericDataAccess.RemoveMany(itemsToRemove);
            
            note.Update(recipient);

            await context.SaveChangesAsync();

            return note;
        }
    }
}

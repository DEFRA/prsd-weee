namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using CuttingEdge.Conditions;
    using Domain.Organisation;
    using EA.Weee.Domain.AatfReturn;
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
            WasteType wasteType,
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
            var wasteTypes = new List<WasteType>();
            if (filter.WasteTypeFilter != null)
            {
                wasteTypes.AddRange(filter.WasteTypeFilter.ToList());
            }

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
            if (filter.SubmittedById.HasValue)
            {
                notes = notes.Where(n => n.Aatf.Id == filter.SubmittedById);
            }
            if (filter.NoteStatusId.HasValue)
            {
                notes = notes.Where(n => n.Status.Value == filter.NoteStatusId);
            }
            if (filter.AllowedStatuses.Any() && !filter.NoteStatusId.HasValue)
            {
                notes = notes.Where(n => allowedStatus.Contains(n.Status.Value));
            }
            if (wasteTypes.Any() && !filter.WasteTypeId.HasValue)
            {
                notes = notes.Where(n => wasteTypes.Contains(n.WasteType.Value));
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
                notes = notes.Where(n => n.WasteType.Value == (WasteType)filter.WasteTypeId.Value);
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

            var count = await notes.Select(n => n.Id).CountAsync();

            var returnNotes = await notes
                .OrderByDescending(n => n.Reference)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new EvidenceNoteResults(returnNotes, count);
        }

        public async Task<IEnumerable<int>> GetComplianceYearsForNotes(List<int> allowedStatuses)
        {
            var notes = context.Notes.Where(n => allowedStatuses.Contains(n.Status.Value));

            var complianceYearsList = await notes.Select(x => x.ComplianceYear).Distinct().OrderByDescending(y => y).ToListAsync();

            return complianceYearsList;
        }

        public async Task<List<Aatf>> GetAatfForAllNotesForComplianceYear(int complianceYear, List<NoteStatus> allowedStatus)
        {
            var status = allowedStatus.Select(e => e.Value);

            return await context.Notes.Where(n => n.ComplianceYear == complianceYear && status.Contains(n.Status.Value))
                .Select(s => s.Aatf)
                .Distinct().ToListAsync();
        }

        public async Task<EvidenceNoteResults> GetNotesToTransfer(Guid recipientOrganisationId,
            List<int> categories,
            List<Guid> excludeEvidenceNotes,
            int complianceYear,
            string searchRef,
            int pageNumber,
            int pageSize)
        {
            var filteredNotes = context.Notes.Where(n => n.RecipientId == recipientOrganisationId &&
                                                         n.NoteType.Value == NoteType.EvidenceNote.Value &&
                                                         n.WasteType == WasteType.HouseHold &&
                                                         n.Status.Value == NoteStatus.Approved.Value &&
                                                         n.ComplianceYear == complianceYear);

            if (!string.IsNullOrWhiteSpace(searchRef))
            {
                var regex = new Regex("^[E|e][1-9]+");
                var formattedReference = regex.Match(searchRef.Trim()).Success ? searchRef.Trim().Remove(0, 1) : searchRef;

                filteredNotes = filteredNotes.Where(n => n.NoteType.Value == NoteType.EvidenceNote.Value && n.Reference.ToString().Equals(formattedReference));
            }

            if (excludeEvidenceNotes.Any())
            {
                filteredNotes = filteredNotes.Where(n => !excludeEvidenceNotes.Contains(n.Id));
            }

            if (categories.Any())
            {
                filteredNotes = filteredNotes.Where(n => n.NoteTonnage.Where(nt => nt.Received != null)
                                    .Select(nt1 => (int)nt1.CategoryId).AsEnumerable().Any(e => categories.Contains(e)));
            }

            var notes = filteredNotes.Include(n =>
                n.NoteTonnage.Select(nt => nt.NoteTransferTonnage.Select(ntt => ntt.TransferNote)));

            var pagedNotes = await notes.OrderByDescending(n => n.Reference)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new EvidenceNoteResults(pagedNotes, notes.Count());
        }

        public async Task<EvidenceNoteResults> GetTransferSelectedNotes(Guid recipientOrganisationId,
            List<Guid> evidenceNotes, List<int> categories)
        {
            var pagedNotes = await context.Notes.Where(n => n.RecipientId == recipientOrganisationId &&
                                                            evidenceNotes.Contains(n.Id) &&
                                                            n.NoteTonnage.Where(nt => nt.Received != null)
                                                                .Select(nt1 => (int)nt1.CategoryId).AsEnumerable().Any(e => categories.Contains(e)))
                .Include(n => n.NoteTonnage.Select(nt => nt.NoteTransferTonnage.Select(ntt => ntt.TransferNote)))
                .ToListAsync();

            return new EvidenceNoteResults(pagedNotes, pagedNotes.Count);
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

            // remove the tonnages that are no longer selected
            var itemsToRemove =
                note.NoteTransferTonnage.Where(ntt => tonnages.All(t => t.NoteTonnageId != ntt.NoteTonnageId)).ToList();

            genericDataAccess.RemoveMany(itemsToRemove);

            // if we are submitting the note then remove any null / zero tonnage values. Some may already be persisted to the database and some may not. 
            if (status == NoteStatus.Submitted)
            {
                for (var idx = note.NoteTransferTonnage.Count() - 1; idx >= 0; idx--)
                {
                    var noteTransferTonnage = note.NoteTransferTonnage.ElementAt(idx);
                    if (noteTransferTonnage.Received == 0.00m || noteTransferTonnage.Received == null)
                    {
                        context.NoteTransferTonnage.Remove(noteTransferTonnage);
                    }
                }
            }

            note.Update(recipient);

            await context.SaveChangesAsync();

            return note;
        }

        public async Task<List<Organisation>> GetRecipientOrganisations(Guid? aatfId,
            int complianceYear,
            List<NoteStatus> allowedStatus,
            List<NoteType> allowedNoteTypes)
        {
            var notes = context.Notes.Where(n => n.ComplianceYear == complianceYear);

            if (aatfId.HasValue)
            {
                var groupedAatf = await context.Aatfs.Where(a => a.Id == aatfId.Value).FirstOrDefaultAsync();

                if (groupedAatf != null)
                {
                    var aatf = await context.Aatfs.Where(a => a.AatfId == groupedAatf.AatfId && a.ComplianceYear == complianceYear).FirstOrDefaultAsync();

                    if (aatf != null)
                    {
                        notes = notes.Where(n => n.AatfId == aatf.Id);
                    }
                    else
                    {
                        return new List<Organisation>();
                    }
                }
                else
                {
                    return new List<Organisation>();
                }
            }

            var status = allowedStatus.Select(e => e.Value);
            var noteTypes = allowedNoteTypes.Select(e => e.Value);

            return await notes.Where(n => status.Contains(n.Status.Value) && noteTypes.Contains(n.NoteType.Value))
                .Select(n => n.Recipient)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Organisation>> GetTransferOrganisations(int complianceYear, List<NoteStatus> allowedStatus, List<NoteType> allowedNoteTypes)
        {
            var status = allowedStatus.Select(e => e.Value);
            var noteTypes = allowedNoteTypes.Select(e => e.Value);

            var notes = context.Notes.Where(n => n.ComplianceYear == complianceYear &&
                                                 n.NoteType.Value == NoteType.TransferNote.Value &&
                                                 status.Contains(n.Status.Value) &&
                                                 noteTypes.Contains(n.NoteType.Value));

            return await notes.Select(n => n.Organisation)
                .Distinct()
                .ToListAsync();
        }

        public Task<bool> HasApprovedWasteHouseHoldEvidence(Guid recipientId, int complianceYear)
        {
            return context.Notes
                .AnyAsync(n => n.ComplianceYear == complianceYear &&
                               n.RecipientId == recipientId &&
                               n.Status.Value == NoteStatus.Approved.Value &&
                               n.WasteType == WasteType.HouseHold &&
                               n.NoteType.Value == NoteType.EvidenceNote.Value);
        }

        public Note DeleteZeroTonnageFromSubmittedTransferNote(Note note, NoteStatus status, NoteType type)
        {
            if (status.Equals(NoteStatus.Submitted) && type.Equals(NoteType.TransferNote))
            {
                var itemsToRemove =
                    note.NoteTransferTonnage.Where(ntt => (ntt.Received == 0.00m || ntt.Received == null));

                genericDataAccess.RemoveMany(itemsToRemove);
            }

            return note;
        }
    }
}

﻿namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Z.EntityFramework.Plus;

    public class EvidenceDataAccess : IEvidenceDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public EvidenceDataAccess(WeeeContext context, 
            IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Note> GetNoteById(Guid id)
        {
            return await context.Notes.FindAsync(id);
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
            var notes = await context.Notes
                .Where(p =>
                    ((!filter.OrganisationId.HasValue || p.Organisation.Id == filter.OrganisationId.Value)
                     && (!filter.AatfId.HasValue || (p.AatfId.HasValue && p.AatfId.Value == filter.AatfId.Value))
                     && (!filter.SchemeId.HasValue || p.Recipient.Id == filter.SchemeId)
                     && (allowedStatus.Contains(p.Status.Value))) && 
                     (filter.SearchRef == null ||
                      (filter.FormattedNoteType > 0 ? 
                       (filter.FormattedNoteType == p.NoteType.Value && filter.FormattedSearchRef == p.Reference.ToString()) : 
                       (filter.FormattedSearchRef == p.Reference.ToString()))))
                .ToListAsync();

            return notes;
        }

        public async Task<IEnumerable<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes)
        {
            var notes = await context.Notes
                .IncludeFilter(n => n.NoteTonnage.Where(nt => 
                    nt.Received.HasValue && categories.Contains((int)nt.CategoryId)))
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

        public async Task<Guid> AddTransferNote(Organisation organisation, Scheme scheme, List<NoteTransferTonnage> transferTonnage, NoteStatus status, string userId)
        {
            var evidenceNote = new Note(organisation,
                scheme,
                userId,
                transferTonnage.ToList());

            if (status.Equals(NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString());
            }

            context.Set<Note>().Add(evidenceNote);

            await context.SaveChangesAsync();

            return evidenceNote.Id;
        }

        public async Task<List<NoteTonnage>> GetTonnageByIds(List<Guid> ids)
        {
            //TODO: Dataaccess test to test for the includes
            return await context.NoteTonnages
                .Include(n => n.Note)
                .Include(n => n.NoteTransferTonnage)
                .Include(n => n.NoteTransferTonnage.Select(nt => nt.TransferNote))
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();
        }
    }
}

namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Prsd.Core.Domain;

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
            var notes = await context.Notes.Where(p =>
                    ((!filter.OrganisationId.HasValue || p.Organisation.Id == filter.OrganisationId.Value)
                     && (!filter.AatfId.HasValue || p.Aatf.Id == filter.AatfId.Value)
                     && (!filter.SchemeId.HasValue || p.Recipient.Id == filter.SchemeId)
                     && (allowedStatus.Contains(p.Status.Value))) && 
                    (filter.FormattedSearchRef == null || (p.Reference.ToString() == filter.FormattedSearchRef)) && (filter.NoteType == null || filter.NoteType.Value == p.NoteType.Value))
                .ToListAsync();

            return notes;
        }
    }
}

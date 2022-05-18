namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Autofac;
    using DataAccess;
    using Domain;
    using Domain.Evidence;
    using Domain.Scheme;
    using Prsd.Core.Autofac;

    public class CommonTestQueryProcessor
    {
        private readonly WeeeContext dbContext;

        public CommonTestQueryProcessor()
        {
            dbContext = ServiceLocator.Container.Resolve<WeeeContext>();
        }

        public Country GetCountryById(Guid id)
        {
            return dbContext.Countries.First(c => c.Id.Equals(id));
        }

        public Note GetEvidenceNoteById(Guid id)
        {
            return dbContext.Notes
                .Include(n => n.Recipient)
                .Include(n => n.NoteTransferTonnage)
                .Include(n => n.NoteTonnage)
                .Include(n => n.NoteTransferCategories)
                .FirstOrDefault(n => n.Id.Equals(id));
        }

        public Scheme GetSchemeById(Guid id)
        {
            return dbContext.Schemes.FirstOrDefault(n => n.Id.Equals(id));
        }

        public Note GetEvidenceNoteByReference(int reference)
        {
            return dbContext.Notes.FirstOrDefault(n => n.Reference == reference);
        }

        public Note GetDraftReturnedEvidenceNote(Guid organisationId, Guid aatfId)
        {
            return dbContext.Notes.FirstOrDefault(n => n.Organisation.Id == organisationId && n.Aatf.Id == aatfId);
        }

        public NoteStatusHistory GetLatestNoteStatusHistoryForNote(Guid noteId)
        {
            return dbContext.NoteStatusHistory.Where(n => n.NoteId.Equals(noteId))
                .OrderByDescending(n => n.ChangedDate).FirstOrDefault();
        }
    }
}

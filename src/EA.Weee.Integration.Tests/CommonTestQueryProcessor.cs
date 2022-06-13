namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Autofac;
    using DataAccess;
    using Domain;
    using Domain.Evidence;
    using Domain.Obligation;
    using Domain.Scheme;
    using Domain.Security;
    using Domain.User;
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

        public UKCompetentAuthority GetEaCompetentAuthority()
        {
            return dbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
        }

        public UKCompetentAuthority GetCompetentAuthorityByName(string name)
        {
            return dbContext.UKCompetentAuthorities.First(c => c.Name.Equals(name));
        }

        public Role GetAdminRole()
        {
            return dbContext.Roles.First(c => c.Description.Equals("Administrator"));
        }

        public ObligationUpload GetObligationUploadById(Guid id)
        {
            return dbContext.ObligationUploads.First(o => o.Id == id);
        }

        public bool CompetentAuthorityUserExists(string userId)
        {
            return dbContext.CompetentAuthorityUsers.Any(u =>
                u.UserId == userId && u.UserStatus.Value == UserStatus.Active.Value);
        }
    }
}

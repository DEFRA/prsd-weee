namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection.Emit;
    using Autofac;
    using Builders;
    using DataAccess;
    using Domain;
    using Domain.Admin;
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
                .FirstOrDefault(n => n.Id.Equals(id));
        }

        public Scheme GetSchemeById(Guid id)
        {
            return dbContext.Schemes.FirstOrDefault(n => n.Id.Equals(id));
        }

        public Scheme GetSchemeByOrganisationId(Guid id)
        {
            return dbContext.Schemes.FirstOrDefault(n => n.OrganisationId.Equals(id));
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

        public Role GetInternalUserRole()
        {
            return dbContext.Roles.First(c => c.Description.Equals("Standard"));
        }

        public ObligationUpload GetObligationUploadById(Guid id)
        {
            return dbContext.ObligationUploads.First(o => o.Id == id);
        }

        public bool CompetentAuthorityUserExists(string userId, Guid roleId)
        {
            return dbContext.CompetentAuthorityUsers.Any(u =>
                u.UserId == userId && u.UserStatus.Value == UserStatus.Active.Value && u.RoleId == roleId);
        }

        public void SetupUserWithRole(string userId, Guid roleId, Guid authorityId)
        {
            var user = dbContext.CompetentAuthorityUsers.First(u => u.UserId == userId);

            if (user != null)
            {
                dbContext.CompetentAuthorityUsers.Remove(user);
            }

            dbContext.SaveChanges();

            CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(userId, authorityId, roleId).Create();
        }
    }
}

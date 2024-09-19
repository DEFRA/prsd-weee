namespace EA.Weee.Integration.Tests
{
    using Autofac;
    using Builders;
    using Core.Helpers;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Domain.Evidence;
    using Domain.Obligation;
    using Domain.Scheme;
    using Domain.Security;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using Newtonsoft.Json.Linq;
    using Prsd.Core.Autofac;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using UserStatus = Domain.User.UserStatus;

    public class CommonTestQueryProcessor
    {
        private readonly WeeeContext dbContext = ServiceLocator.Container.Resolve<WeeeContext>();

        public Country GetCountryById(Guid id)
        {
            return dbContext.Countries.First(c => c.Id.Equals(id));
        }

        public async Task<Country> GetCountryByNameAsync(string name)
        {
            return await dbContext.Countries.FirstAsync(c => c.Name.Equals(name));
        }

        public Organisation GetOrganisationById(Guid id)
        {
            return dbContext.Organisations.First(c => c.Id.Equals(id));
        }

        public DirectRegistrant GetDirectRegistrantByOrganisationId(Guid organisationId)
        {
            return dbContext.DirectRegistrants.First(c => c.OrganisationId.Equals(organisationId));
        }

        public DirectProducerSubmission GetDirectProducerSubmissionById(Guid directSubmissionId)
        {
            return dbContext.DirectProducerSubmissions.First(c => c.Id == directSubmissionId);
        }

        public DirectProducerSubmissionHistory CurrentSubmissionHistoryForComplianceYear(Guid directRegistrantId, int year)
        {
            return dbContext.DirectProducerSubmissionHistories.First(c => c.DirectProducerSubmission.DirectRegistrantId == directRegistrantId && c.DirectProducerSubmission.ComplianceYear == year);
        }

        public List<AdditionalCompanyDetails> GetAdditionalDetailsByRegistrantId(Guid directRegistrantId, OrganisationAdditionalDetailsType type)
        {
            return dbContext.AdditionalCompanyDetails.Where(c => c.DirectRegistrant.Id.Equals(directRegistrantId) && c.Type.Value == type.Value).ToList();
        }

        public Note GetEvidenceNoteById(Guid id)
        {
            return dbContext.Notes
                .Include(n => n.Recipient.Schemes)
                .Include(n => n.Organisation)
                .Include(n => n.Aatf)
                .Include(n => n.NoteTonnage)
                .FirstOrDefault(n => n.Id.Equals(id));
        }

        public Note GetTransferEvidenceNoteById(Guid id)
        {
            return dbContext.Notes
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

        public Guid GetBalancingSchemeId()
        {
            return dbContext.ProducerBalancingSchemes.First(c => c.Organisation != null).Organisation.Id;
        }

        public OrganisationTransaction GetOrganisationTransactionForUser(string userId)
        {
            return dbContext.OrganisationTransactions.FirstOrDefault(c => c.UserId.Equals(userId));
        }

        public int GetOrganisationTransactionCountForUser(string userId)
        {
            return dbContext.OrganisationTransactions.Count(c => c.UserId.Equals(userId));
        }

        public void UpdateCurrentProducerSubmission(Guid directRegistrantSubmissionId, Guid directRegistrantSubmissionHistoryId)
        {
            dbContext.Database.ExecuteSqlCommand(
                $"UPDATE [Producer].DirectProducerSubmission Set CurrentSubmissionId = '{directRegistrantSubmissionHistoryId}' WHERE Id = '{directRegistrantSubmissionId}'");
        }

        public async Task DeleteAllOrganisationTransactionsAsync()
        {
            var organisationTransactions = dbContext.OrganisationTransactions.ToList();

            dbContext.OrganisationTransactions.RemoveRange(organisationTransactions);

            await dbContext.SaveChangesAsync();
        }

        public void DeleteAllAdditionalCompanyDetails()
        {
            var additionalContactDetails = dbContext.AdditionalCompanyDetails.ToList();

            dbContext.AdditionalCompanyDetails.RemoveRange(additionalContactDetails);

            dbContext.SaveChanges();
        }

        public List<OrganisationUser> GetOrganisationForUser(string userId)
        {
            return dbContext.OrganisationUsers.Where(ou => ou.UserId == userId).ToList();
        }
        public void SetupUserWithRole(string userId, string role, CompetentAuthority authority)
        {
            var user = dbContext.CompetentAuthorityUsers.FirstOrDefault(u => u.UserId == userId);
            var authorityDisplayName = authority.ToDisplayString();
            var competentAuthority = dbContext.UKCompetentAuthorities.First(c => c.Name.Equals(authorityDisplayName));
            var userRole = dbContext.Roles.First(c => c.Description.Equals(role));

            if (user != null)
            {
                dbContext.Database.ExecuteSqlCommand($"DELETE FROM [Admin].CompetentAuthorityUser WHERE UserId = '{userId}'");
            }

            CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(userId, competentAuthority.Id, userRole.Id).Create();
        }
    }
}

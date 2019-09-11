namespace EA.Weee.Email
{
    using Domain.Organisation;
    using System.Threading.Tasks;

    using EA.Weee.Core.Shared;

    public interface IWeeeEmailService
    {
        Task<bool> SendActivateUserAccount(string emailAddress, string activationUrl);

        Task<bool> SendPasswordResetRequest(string emailAddress, string passwordResetUrl);

        Task<bool> SendOrganisationUserRequest(string emailAddress, string organisationName, string userName);

        Task<bool> SendOrganisationUserRequestCompleted(OrganisationUser organisationUser, bool activeUsers);

        Task<bool> SendSchemeMemberSubmitted(string emailAddress, string schemeName, int complianceYear, int numberOfWarnings);

        Task<bool> SendSchemeDataReturnSubmitted(string emailAddress, string schemeName, int complianceYear, int quarter, bool isResubmission);

        Task<bool> SendInternalUserAccountActivated(string emailAddress, string userFullName, string userEmailAddress, string viewUserLink);

        Task<bool> SendTestEmail(string emailAddress);

        Task<bool> SendOrganisationContactDetailsChanged(string emailAddress, string name, EntityType entityType);

        Task<bool> SendOrganisationUserRequestToEA(string emailAddress, string organisationName, string userName);
    }
}

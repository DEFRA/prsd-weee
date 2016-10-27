namespace EA.Weee.Email
{
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IWeeeEmailService
    {
        Task<bool> SendActivateUserAccount(string emailAddress, string activationUrl);

        Task<bool> SendPasswordResetRequest(string emailAddress, string passwordResetUrl);

        Task<bool> SendOrganisationUserRequest(string emailAddress, string organisationName);

        Task<bool> SendOrganisationUserRequestCompleted(OrganisationUser organisationUser);

        Task<bool> SendSchemeMemberSubmitted(string emailAddress, string schemeName, int complianceYear, int numberOfWarnings);

        Task<bool> SendSchemeDataReturnSubmitted(string emailAddress, string schemeName, int complianceYear, int quarter, bool isResubmission);

        Task<bool> SendInternalUserAccountActivated(string emailAddress, string userFullName, string userEmailAddress, string viewUserLink);

        Task<bool> SendTestEmail(string emailAddress);
    }
}

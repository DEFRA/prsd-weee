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
    }
}

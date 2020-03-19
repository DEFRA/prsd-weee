namespace EA.Weee.Api.Client.Actions
{
    using Entities;
    using System.Threading.Tasks;

    public interface IUnauthenticatedUser
    {
        Task<string> CreateInternalUserAsync(InternalUserCreationData userCreationData, string accessToken);

        Task<string> CreateExternalUserAsync(ExternalUserCreationData userCreationData, string accessToken);

        Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedAccountData, string token);

        Task<bool> ResetPasswordAsync(PasswordResetData passwordResetData, string accessToken);

        Task<bool> ResendActivationEmail(string accessToken, string activationBaseUrl);

        Task<bool> ResendActivationEmailByUserId(string userId, string emailAddress, string activationBaseUrl, string token);

        Task<PasswordResetRequestResult> ResetPasswordRequestAsync(PasswordResetRequest passwordResetRequest, string token);

        Task<bool> IsPasswordResetTokenValidAsync(PasswordResetData model, string accessToken);
    }
}
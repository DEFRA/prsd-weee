namespace EA.Weee.Api.Client.Actions
{
    using Entities;
    using System.Threading.Tasks;

    public interface IUnauthenticatedUser
    {
        Task<string> CreateInternalUserAsync(InternalUserCreationData userCreationData);
        
        Task<string> CreateExternalUserAsync(ExternalUserCreationData userCreationData);
        
        Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedAccountData);
        
        Task<string> GetUserAccountActivationTokenAsync(string accessToken);
        
        Task<bool> ResetPasswordAsync(PasswordResetData passwordResetData);
        
        Task<bool> ResendActivationEmail(string accessToken, string activationBaseUrl);

        Task<bool> ResendActivationEmailByUserId(string userId, string emailAddress, string activationBaseUrl);

        Task<PasswordResetRequestResult> ResetPasswordRequestAsync(PasswordResetRequest passwordResetRequest);

        Task<bool> IsPasswordResetTokenValidAsync(PasswordResetData model);
    }
}
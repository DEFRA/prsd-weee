namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;
    using Entities;

    public interface IUnauthenticatedUser
    {
        Task<string> CreateInternalUserAsync(InternalUserCreationData userCreationData);
        
        Task<string> CreateExternalUserAsync(ExternalUserCreationData userCreationData);
        
        Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedAccountData);
        
        Task<string> GetUserAccountActivationTokenAsync(string accessToken);
        
        Task<PasswordResetResult> ResetPasswordAsync(PasswordResetData passwordResetData);
        
        Task<bool> ResendActivationEmail(string accessToken, string activationBaseUrl);

        Task<bool> ResetPasswordRequestAsync(string emailAddress);
    }
}
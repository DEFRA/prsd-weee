namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;
    using Entities;

    public interface IUnauthenticatedUser
    {
        Task<string> CreateUserAsync(UserCreationData userCreationData);
        Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedAccountData);
        Task<string> GetUserAccountActivationTokenAsync(string accessToken);
        Task<bool> ResetPasswordRequestAsync(string emailAddress);
    }
}
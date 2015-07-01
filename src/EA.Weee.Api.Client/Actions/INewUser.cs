namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;
    using Entities;

    public interface INewUser
    {
        Task<string> CreateUserAsync(UserCreationData userCreationData);
        Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedAccountData);
        Task<string> GetUserAccountActivationTokenAsync(string accessToken);
    }
}
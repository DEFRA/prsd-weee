namespace EA.Weee.RequestHandlers.Users.UserPasswordHistories
{
    using EA.Weee.Requests.Users;
    using System.Threading.Tasks;

    public interface IUserPasswordHistoriesDataAccess
    {
        Task<bool> AddUserPasswordHistories(string userId, string passwordHash);

        Task<bool> ValidateUserPasswordHistories(GetUserPasswordHistory getUserPasswordHistory);
    }
}

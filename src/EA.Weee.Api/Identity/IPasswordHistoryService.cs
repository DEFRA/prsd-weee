namespace EA.Weee.Api.Identity
{
    using EA.Weee.DataAccess.Identity;
    using System.Threading.Tasks;

    public interface IPasswordHistoryService
    {
        Task<bool> IsPasswordPreviouslyUsed(ApplicationUser user, string newPassword);

        Task<bool> AddUserPasswordHistories(string userId, string passwordHash);
    }
}

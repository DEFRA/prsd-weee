namespace EA.Weee.Api.Identity
{
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain.User;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class PasswordHistoryService : IPasswordHistoryService
    {
        private readonly WeeeContext context;
        private readonly PasswordHasher hasher;

        public PasswordHistoryService(WeeeContext context)
        {
            this.context = context;
            hasher = new PasswordHasher();
        }

        /// <summary>
        /// This method checks whether the user is using the last 5 passwords.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> IsPasswordPreviouslyUsed(ApplicationUser user, string newPassword)
        {
            // Check current password
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                var currentResult = hasher.VerifyHashedPassword(user.PasswordHash, newPassword);

                if (currentResult != PasswordVerificationResult.Failed)
                {
                    return true;
                }
            }

            // Check previous 5 passwords
            string userId = user.Id.ToString();
            var previousPasswords = await context.PasswordHistories
                                                 .Where(x => x.UserId == userId)
                                                 .OrderByDescending(x => x.CreatedDate)
                                                 .Take(5)
                                                 .ToListAsync();

            return previousPasswords.Any(x => hasher.VerifyHashedPassword(x.PasswordHash, newPassword) != PasswordVerificationResult.Failed);
        }

        /// <summary>
        /// This method to add user password histories table data.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public async Task<bool> AddUserPasswordHistories(string userId, string passwordHash)
        {
            var passwordHistories = new PasswordHistory()
            {
                UserId = userId,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.UtcNow
            };

            context.PasswordHistories.Add(passwordHistories);

            await context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// This method is to update User Password change date and Password Histories table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePasswordChangeDateAndHistory(string userId, string passwordHash)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return false;
            }

            var currentDate = DateTime.UtcNow;

            // Update password changed date
            user.PasswordChangedDate = currentDate;

            // Save previous password hash in history
            var passwordHistory = new PasswordHistory
            {
                UserId = userId,
                PasswordHash = passwordHash,
                CreatedDate = currentDate
            };

            context.PasswordHistories.Add(passwordHistory);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
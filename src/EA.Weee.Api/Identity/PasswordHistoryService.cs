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
            var previousPasswords = await context.PasswordHistories
                                                 .Where(x => x.UserId == user.Id)
                                                 .OrderByDescending(x => x.CreatedDate)
                                                 .Take(5)
                                                 .ToListAsync();

            return previousPasswords.Any(x => hasher.VerifyHashedPassword(x.PasswordHash, newPassword) != PasswordVerificationResult.Failed);
        }

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
    }
}
namespace EA.Weee.RequestHandlers.Users.UserPasswordHistories
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.User;
    using EA.Weee.Requests.Users;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserPasswordHistoriesDataAccess : IUserPasswordHistoriesDataAccess
    {
        private readonly WeeeContext context;
        private readonly IPasswordHasher passwordHasher;

        public UserPasswordHistoriesDataAccess(WeeeContext context, IPasswordHasher passwordHasher)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
        }

        public async Task<bool> AddUserPasswordHistories(string userId, string passwordHash)
        {
            var userPasswordHistories = new UserPasswordHistories(userId, passwordHash);

            context.UserPasswordHistories.Add(userPasswordHistories);

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateUserPasswordHistories(GetUserPasswordHistory getUserPassword)
        {
            var histories = await context.UserPasswordHistories
                                      .Where(x => x.UserId == getUserPassword.UserId.ToString())
                                      .OrderByDescending(x => x.CreatedDate)
                                      .ThenByDescending(x => x.Id)
                                      .Take(5)
                                      .ToListAsync();

            foreach (var history in histories)
            {
                var result = passwordHasher.VerifyHashedPassword(history.PasswordHash, getUserPassword.Password);

                if (result != PasswordVerificationResult.Failed)
                {
                    return true;
                }
            }

            context.UserPasswordHistories.Add(new UserPasswordHistories
            {
                UserId = getUserPassword.UserId.ToString(),
                PasswordHash = getUserPassword.Password,
                CreatedDate = DateTime.UtcNow
            });

            context.SaveChanges();

            return false;
        }
    }
}

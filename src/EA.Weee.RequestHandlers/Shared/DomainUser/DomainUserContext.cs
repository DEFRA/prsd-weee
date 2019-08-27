﻿namespace EA.Weee.RequestHandlers.Shared.DomainUser
{
    using DataAccess;
    using Domain.User;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for fetching objects representing domain users.
    /// </summary>
    public class DomainUserContext : IDomainUserContext
    {
        private readonly WeeeContext context;

        public DomainUserContext(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Fetches the domain user representing the current application user.
        /// </summary>
        /// <returns></returns>
        public Task<User> GetCurrentUserAsync()
        {
            string userId = context.GetCurrentUser();

            return GetUserAsync(userId);
        }

        /// <summary>
        /// Fetches the domain user representing the specified application user.
        /// </summary>
        /// <param name="userId">The ID of the application user.</param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string userId)
        {
            return await context.Users.FindAsync(userId);
        }
    }
}

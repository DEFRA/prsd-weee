namespace EA.Weee.Api.IdSrv
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.Identity;
    using Identity;
    using IdentityServer3.Core.Models;
    using Microsoft.AspNet.Identity;

    public class UserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        private readonly ApplicationUserManager userManager;

        public UserService(ApplicationUserManager userMgr, WeeeContext context)
            : base(userMgr, context)
        {
            userManager = userMgr;
        }

        protected override async Task<AuthenticateResult> PostAuthenticateLocalAsync(ApplicationUser user, SignInMessage message)
        {
            // Update the last login date on successful authentication
            user.LastLoginDate = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return await base.PostAuthenticateLocalAsync(user, message);
        }
    }
}
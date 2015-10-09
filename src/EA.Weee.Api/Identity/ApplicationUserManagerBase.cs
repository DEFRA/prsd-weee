namespace EA.Weee.Api.Identity
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// A base class which raises events on an IAuditSecurityAuditor relating to user creation,
    /// user updates, account verification and claim issueing.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public abstract class ApplicationUserManagerBase<TUser> : UserManager<TUser> where TUser : class, Microsoft.AspNet.Identity.IUser<string>
    {
        private readonly ISecurityEventAuditor auditSecurityEventService;

        public ApplicationUserManagerBase(IUserStore<TUser> store, ISecurityEventAuditor auditSecurityEventService)
            : base(store)
        {
            this.auditSecurityEventService = auditSecurityEventService;
        }

        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            IdentityResult result = await base.CreateAsync(user);

            await auditSecurityEventService.UserCreated(user);

            return result;
        }

        public override async Task<IdentityResult> UpdateAsync(TUser user)
        {
            IdentityResult result = await base.UpdateAsync(user);

            await auditSecurityEventService.UserUpdated(user);

            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            IdentityResult result = await base.ResetPasswordAsync(userId, token, newPassword);

            await auditSecurityEventService.PasswordReset(userId);

            return result;
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            IdentityResult result = await base.ConfirmEmailAsync(userId, token);

            await auditSecurityEventService.EmailConfirmed(userId);

            return result;
        }
    }
}
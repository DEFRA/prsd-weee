namespace EA.Weee.Api.Identity
{
    using DataAccess.Identity;
    using EA.Weee.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security.DataProtection;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ApplicationUserManager : ApplicationUserManagerBase<ApplicationUser>
    {
        private readonly ConfigurationService configurationService;
        private readonly WeeeContext context;

        public ApplicationUserManager(
            IUserStore<ApplicationUser> store,
            ISecurityEventAuditor auditSecurityEventService,
            IDataProtectionProvider dataProtectionProvider,
            ConfigurationService configurationService,
            WeeeContext context)
            : base(store, auditSecurityEventService)
        {
            this.configurationService = configurationService;
            this.context = context;

            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            IDataProtector dataProtector = dataProtectionProvider.Create("ASP.NET Identity");

            var userTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);
            userTokenProvider.TokenLifespan = TimeSpan.FromHours(24);

            UserTokenProvider = userTokenProvider;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            SetEmailConfirmedIfRequired(user);

            return await base.CreateAsync(user);
        }

        public override async Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            var claims = await base.GetClaimsAsync(userId);

            var user = await Store.FindByIdAsync(userId);
            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue));
            }

            claims.Add(new Claim(ClaimTypes.Name, string.Format("{0} {1}", user.FirstName, user.Surname)));

            // Load WEEE-specific claims representing access to organisations.
            List<Guid> organisationIds = await context.OrganisationUsers
                            .Where(ou => ou.UserId == userId)
                            .Where(ou => ou.UserStatus.Value == (int)UserStatus.Active)
                            .Select(ou => ou.OrganisationId)
                            .ToListAsync();

            foreach (Guid organisationId in organisationIds)
            {
                claims.Add(new Claim(WeeeClaimTypes.OrganisationAccess, organisationId.ToString()));
            }

            // Load WEEE-specific claims representing access to schemes. 
            List<Guid> schemeIds = await context.Schemes
                .Where(s => organisationIds.Contains(s.OrganisationId))
                .Select(s => s.Id)
                .ToListAsync();

            foreach (Guid schemeId in schemeIds)
            {
                claims.Add(new Claim(WeeeClaimTypes.SchemeAccess, schemeId.ToString()));
            }

            return claims;
        }

        private void SetEmailConfirmedIfRequired(ApplicationUser user)
        {
            // We exclude verify email where the environment is set to development.
            if (configurationService.CurrentConfiguration.Environment.Equals("Development",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                var verificationDomains = new List<string>();

                //List not empty
                if (
                    !string.IsNullOrWhiteSpace(
                        configurationService.CurrentConfiguration.VerificationEmailBypassDomains))
                {
                    // Get the domains for which email verification is still required.
                    verificationDomains =
                        configurationService.CurrentConfiguration.VerificationEmailBypassDomains.Split(
                            new[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                var domainStarts = user.Email.LastIndexOf("@");
                var excludeThisEmail = verificationDomains.Any(d => user.Email.Substring(domainStarts).Contains(d));

                if (excludeThisEmail)
                {
                    user.EmailConfirmed = true;
                }
            }
        }
    }
}
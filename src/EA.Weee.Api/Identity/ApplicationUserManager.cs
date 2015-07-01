namespace EA.Weee.Api.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security.DataProtection;
    using Services;

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly ConfigurationService configurationService;

        public ApplicationUserManager(IUserStore<ApplicationUser> store,
            IDataProtectionProvider dataProtectionProvider,
            ConfigurationService configurationService)
            : base(store)
        {
            this.configurationService = configurationService;
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

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            EmailService = new EmailService();

            UserTokenProvider =
                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            SetEmailConfirmedIfRequired(user);

            return await base.CreateAsync(user);
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
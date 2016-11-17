namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Configuration;
    using DataAccess;
    using Domain.Admin;
    using Domain.User;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Weee.Security;

    public class AddCompetentAuthorityUserHandler : IRequestHandler<AddCompetentAuthorityUser, Guid>
    {
        private readonly WeeeContext context;
        private readonly ITestUserEmailDomains testInternalUserEmailDomains;

        public AddCompetentAuthorityUserHandler(WeeeContext context, ITestUserEmailDomains testInternalUserEmailDomains)
        {
            this.context = context;
            this.testInternalUserEmailDomains = testInternalUserEmailDomains;
        }

        public async Task<Guid> HandleAsync(AddCompetentAuthorityUser message)
        {
            var userId = message.UserId;
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", userId));
            }
            // get competent authority from email address
            int indexofat = user.Email.IndexOf('@');
            string email = user.Email.Substring(indexofat + 1);
            string authorityName = AuthorityName(email);

            var competentAuthority =
                await context.UKCompetentAuthorities.FirstOrDefaultAsync(c => c.Abbreviation == authorityName);
            if (competentAuthority == null)
            {
                throw new InvalidOperationException(string.Format("Could not find the competent authority with name: {0}", authorityName));
            }

            var standardRole = await context.Roles.SingleOrDefaultAsync(r => r.Name == Roles.InternalUser.ToString());
            if (standardRole == null)
            {
                throw new InvalidOperationException(string.Format("Could not find the role name with: {0}", Roles.InternalUser));
            }

            CompetentAuthorityUser competentAuthorityUser = new CompetentAuthorityUser(user.Id, competentAuthority.Id, UserStatus.Pending, standardRole);
            context.CompetentAuthorityUsers.Add(competentAuthorityUser);

            await context.SaveChangesAsync();
            return competentAuthorityUser.Id;
        }

        private string AuthorityName(string domain)
        {
            string authorityName;

            switch (domain)
            {
                case "environment-agency.gov.uk":
                    authorityName = "EA";
                    break;

                case "cyfoethnaturiolcymru.gov.uk":
                case "naturalresourceswales.gov.uk":
                    authorityName = "NRW";
                    break;

                case "sepa.org.uk":
                    authorityName = "SEPA";
                    break;

                case "daera-ni.gov.uk":
                    authorityName = "NIEA";
                    break;

                default:
                    {
                        if (testInternalUserEmailDomains.UserTestModeEnabled && IsDomainAllowedForTestUser(domain))
                        {
                            authorityName = "EA";
                            break;
                        }

                        string errorMessage = string.Format(
                            "The authority could not be determined from the domain name \"{0}\".",
                            domain);

                        throw new InvalidOperationException(errorMessage);
                    }
            }

            return authorityName;
        }

        private bool IsDomainAllowedForTestUser(string domain)
        {
            foreach (string allowedTestDomain in testInternalUserEmailDomains.Domains)
            {
                if (string.Equals(allowedTestDomain, domain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

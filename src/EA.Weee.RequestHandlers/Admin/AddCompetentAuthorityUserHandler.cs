namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Configuration;
    using DataAccess;
    using Domain;
    using Domain.Admin;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;

    public class AddCompetentAuthorityUserHandler : IRequestHandler<AddCompetentAuthorityUser, Guid>
    {
        private readonly WeeeContext context;
        public IConfigurationManagerWrapper Configuration { get; set; }

        public AddCompetentAuthorityUserHandler(WeeeContext dbContext)
        {
            context = dbContext;
            Configuration = new ConfigurationManagerWrapper();
        }

        public async Task<Guid> HandleAsync(AddCompetentAuthorityUser message)
        {
            var userId = message.UserId;
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());
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

            CompetentAuthorityUser competentAuthorityUser = new CompetentAuthorityUser(user.Id, competentAuthority.Id, UserStatus.Pending);
            context.CompetentAuthorityUsers.Add(competentAuthorityUser);
            
            await context.SaveChangesAsync();
            return competentAuthorityUser.Id;
        }

        private string AuthorityName(string email)
        {
            string authorityName = string.Empty;
            string internalUsersTestMode = Configuration.HasKey("Weee.InternalUsersTestMode") ? Configuration.GetKeyValue("Weee.InternalUsersTestMode") : null;

            switch (email)
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

                case "doeni.gov.uk":
                    authorityName = "NIEA";
                    break;

                default:
                    if (internalUsersTestMode != null && internalUsersTestMode.Equals("true"))
                    {
                        return "EA";
                    }
                    break;
            }
            return authorityName;
        }
    }
}

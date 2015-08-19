namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Admin;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;

    public class AddCompetentAuthorityUserHandler : IRequestHandler<AddCompetentAuthorityUser, Guid>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public AddCompetentAuthorityUserHandler(WeeeContext dbContext, IUserContext userContext)
        {
            context = dbContext;
            this.userContext = userContext;
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
                throw new ArgumentException(string.Format("Could not find the competent authority with name: {0}", authorityName));
            }

            CompetentAuthorityUser competentAuthorityUser = new CompetentAuthorityUser(user.Id, competentAuthority.Id, UserStatus.Pending);
            context.CompetentAuthorityUsers.Add(competentAuthorityUser);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            
            return competentAuthorityUser.Id;
        }

        private string AuthorityName(string email)
        {
            string authorityName = string.Empty;
            string internalusersMode = ConfigurationManager.AppSettings["Weee.InternalUsersMode"];
            if (internalusersMode.Equals("true"))
            {
                return "EA";
            }

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
            }
            return authorityName;
        }
    }
}

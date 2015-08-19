namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FindMatchingUsersDataAccess : IFindMatchingUsersDataAccess
    {
        private readonly WeeeContext context;

        public FindMatchingUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<UserSearchData[]> GetCompetentAuthorityUsers()
        {
            var competentAuthorityUsers = await(
                    from u in context.Users
                    join cu in context.CompetentAuthorityUsers on u.Id equals cu.UserId into caUsers
                    from caUser in caUsers
                    join ca in context.UKCompetentAuthorities on caUser.CompetentAuthorityId equals ca.Id
                    select new UserSearchData
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Id = u.Id,
                        LastName = u.Surname,
                        OrganisationName = ca.Abbreviation,
                        Status = (UserStatus)caUser.UserStatus.Value
                    }).ToArrayAsync();

            return competentAuthorityUsers;
        }

        public async Task<UserSearchData[]> GetOrganisationUsers()
        {
            var organisationsUsers = await(
                    from u in context.Users
                    join ou in context.OrganisationUsers on u.Id equals ou.UserId into idOrgUsers
                    from orgUser in idOrgUsers
                    join org in context.Organisations on orgUser.OrganisationId equals org.Id
                    select new UserSearchData
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Id = u.Id,
                        LastName = u.Surname,
                        OrganisationName = org.Name ?? org.TradingName,
                        Status = (UserStatus)orgUser.UserStatus.Value
                    }).ToArrayAsync();

            return organisationsUsers;
        }
    }
}

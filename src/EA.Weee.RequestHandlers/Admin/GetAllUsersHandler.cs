namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using userStatus = Core.Shared.UserStatus;

    public class GetAllUsersHandler : IRequestHandler<GetAllUsers, List<UserSearchData>>
    {
          private readonly WeeeContext context;
   
          public GetAllUsersHandler(WeeeContext context)
            {
                this.context = context;
            }

        public async Task<List<UserSearchData>> HandleAsync(GetAllUsers message)
        {
            var users = new List<UserSearchData>();
            // organisation users
            var organisationsUsers = await GetOrganisationUsers();
            users.AddRange(organisationsUsers.Select(user => user).ToList());

            //internal users
            var competentAuthorityUsers = await GetCompetentAuthorityUsers();
            users.AddRange(competentAuthorityUsers.Select(interaluser => interaluser).ToList());
            return users.OrderBy(u => u.FullName).ToList();
        }

        private async Task<List<UserSearchData>> GetCompetentAuthorityUsers()
        {
            var competentAuthorityUsers = await(from u in context.Users
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
                    Status = (userStatus)caUser.UserStatus.Value
                }).ToListAsync();
            return competentAuthorityUsers;
        }

        private async Task<List<UserSearchData>> GetOrganisationUsers()
        {
            var organisationsUsers = await(from u in context.Users
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
                    Status = (userStatus)orgUser.UserStatus.Value
                }).ToListAsync();
            return organisationsUsers;
        }
    }
}

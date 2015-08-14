namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared.Paging;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using userStatus = Core.Shared.UserStatus;

    internal class FindMatchingUsersHandler : IRequestHandler<FindMatchingUsers, UserSearchDataResult>
    {
          private readonly WeeeContext context;
   
          public FindMatchingUsersHandler(WeeeContext context)
            {
                this.context = context;
            }

        public async Task<UserSearchDataResult> HandleAsync(FindMatchingUsers query)
        {
            var organisationsUsers = await GetOrganisationUsers();
      
            //internal users
            var competentAuthorityUsers = await GetCompetentAuthorityUsers();
            var totalUsersData = organisationsUsers.Concat(competentAuthorityUsers).OrderBy(u => u.FullName).ToList();
            if (query.Paged)
            {
               IList<UserSearchData> pagedMatchingUsersData =
                    totalUsersData.Skip((query.Page - 1) * query.UsersPerPage)
                        .Take(query.UsersPerPage)
                        .ToList();
                //IPagedList<UserSearchData> pagedList = pagedMatchingUsersData.ToPagedList(query.Page, query.UsersPerPage, totalUsersData.Count);
               return new UserSearchDataResult(pagedMatchingUsersData, totalUsersData.Count);
            }
            //(totalUsersData.ToPagedList(1, 1, totalUsersData.Count)
            return new UserSearchDataResult(totalUsersData, totalUsersData.Count);
        }

        private async Task<UserSearchData[]> GetCompetentAuthorityUsers()
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
                }).ToArrayAsync();
            return competentAuthorityUsers;
        }

        private async Task<UserSearchData[]> GetOrganisationUsers()
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
                }).ToArrayAsync();
            return organisationsUsers;
        }
    }
}

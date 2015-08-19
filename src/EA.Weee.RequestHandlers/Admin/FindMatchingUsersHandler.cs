namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using Core.Shared.Paging;
    using DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using UserStatus = Core.Shared.UserStatus;

    internal class FindMatchingUsersHandler : IRequestHandler<FindMatchingUsers, UserSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFindMatchingUsersDataAccess dataAccess;

        public FindMatchingUsersHandler(IWeeeAuthorization authorization, IFindMatchingUsersDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<UserSearchDataResult> HandleAsync(FindMatchingUsers query)
        {
            authorization.EnsureCanAccessInternalArea();

            var organisationsUsers = await dataAccess.GetOrganisationUsers();
            var competentAuthorityUsers = await dataAccess.GetCompetentAuthorityUsers();

            var totalUsersData = organisationsUsers.Concat(competentAuthorityUsers).OrderBy(u => u.FullName).ToList();
            
            if (query.Paged)
            {
                IList<UserSearchData> pagedMatchingUsersData =
                     totalUsersData.Skip((query.Page - 1) * query.UsersPerPage)
                         .Take(query.UsersPerPage)
                         .ToList();
                return new UserSearchDataResult(pagedMatchingUsersData, totalUsersData.Count);
            }

            return new UserSearchDataResult(totalUsersData, totalUsersData.Count);
        }
    }
}

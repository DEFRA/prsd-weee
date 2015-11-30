namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
 
    internal class FindMatchingUsersHandler : IRequestHandler<FindMatchingUsers, UserSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFindMatchingUsersDataAccess dataAccess;

        private static readonly StatusComparer statusComparer = new StatusComparer();

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

            List<UserSearchData> results = new List<UserSearchData>();
            results.AddRange(organisationsUsers);
            results.AddRange(competentAuthorityUsers);

            IOrderedEnumerable<UserSearchData> orderedResults;
            switch (query.Ordering)
            {
                case FindMatchingUsers.OrderBy.FullNameAscending:
                    orderedResults = results
                        .OrderBy(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                case FindMatchingUsers.OrderBy.FullNameDescending:
                    orderedResults = results
                        .OrderByDescending(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                case FindMatchingUsers.OrderBy.OrganisationAscending:
                    orderedResults = results
                        .OrderBy(u => u.OrganisationName)
                        .ThenBy(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                case FindMatchingUsers.OrderBy.OrganisationDescending:
                    orderedResults = results
                        .OrderByDescending(u => u.OrganisationName)
                        .ThenBy(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                case FindMatchingUsers.OrderBy.StatusAscending:
                    orderedResults = results
                        .OrderBy(u => u.Status, statusComparer)
                        .ThenBy(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                case FindMatchingUsers.OrderBy.StatusDescending:
                    orderedResults = results
                        .OrderByDescending(u => u.Status, statusComparer)
                        .ThenBy(u => u.FullName)
                        .ThenBy(u => u.Id);
                    break;

                default:
                    throw new NotSupportedException();
            }

            IList<UserSearchData> pagedResults = orderedResults
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new UserSearchDataResult(pagedResults, results.Count());
        }

        private class StatusComparer : IComparer<UserStatus>
        {
            private Dictionary<UserStatus, int> ranks = new Dictionary<UserStatus, int>()
            {
                { UserStatus.Active, 0},
                { UserStatus.Inactive, 1},
                { UserStatus.Pending, 2},
                { UserStatus.Rejected, 3},
            };

            public int Compare(UserStatus x, UserStatus y)
            {
                int rankX = ranks[x];
                int rankY = ranks[y];

                return rankX.CompareTo(rankY);
            }
        }
    }
}

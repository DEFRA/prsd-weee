namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;
    using Shared.Paging;

    public class UserSearchDataResult
    {
        public IList<UserSearchData> Results { get; private set; }
        public int UsersCount { get; private set; }

        public UserSearchDataResult(IList<UserSearchData> results, int usersCount)
        {
            Results = results;
            UsersCount = usersCount;
        }

        protected UserSearchDataResult()
        {
            Results = new PagedList<UserSearchData>();
        }
    }
}

namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;
    
    public class UserSearchDataResult
    {
        public IList<UserSearchData> Results { get; private set; }
        public int UsersCount { get; private set; }

        public UserSearchDataResult(IList<UserSearchData> results, int usersCount)
        {
            Results = results;
            UsersCount = usersCount;
        }
    }
}

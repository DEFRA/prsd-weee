namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
     
    public class FindMatchingUsers : IRequest<UserSearchDataResult>
    {
         public bool Paged { get; private set; }

        public int Page { get; private set; }

        public int UsersPerPage { get; set; }

        public FindMatchingUsers(int? page = null, int? usersPerPage = null)
        {
            if (page.HasValue && usersPerPage.HasValue)
            {
                Paged = true;
                Page = page.Value;
                UsersPerPage = usersPerPage.Value;
            }
        }
    }
}

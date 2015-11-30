namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;
     
    public class FindMatchingUsers : IRequest<UserSearchDataResult>
    {
        /// <summary>
        /// Defines which page of results is returned.
        /// Specifying a value that number that exceeds the number of pages will
        /// result in the query returning an empty page.
        /// The first page is 1.
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Defines how many users are returned for the requested page of results.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Defines how the results will be ordered.
        /// </summary>
        public OrderBy Ordering { get; private set; }

        public FindMatchingUsers(int pageNumber, int pageSize, OrderBy ordering)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            PageNumber = pageNumber;
            PageSize = pageSize;
            Ordering = ordering;
        }

        public enum OrderBy
        {
            /// <summary>
            /// Order results by full name from A to Z, where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            FullNameAscending,

            /// <summary>
            /// Order results by full name from Z to A, where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            FullNameDescending,

            /// <summary>
            /// Order results by organisation name from A to Z.
            /// Results within the same organisation will be sorted by full name from A to Z,
            /// where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            OrganisationAscending,

            /// <summary>
            /// Order results by organisation name from Z to A.
            /// Results within the same organisation will be sorted by full name from A to Z,
            /// where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            OrganisationDescending,

            /// <summary>
            /// Order results by status in the following order: "Active", "Inactive", "Pending", "Rejected".
            /// Results with the same status will be sorted by full name from A to Z,
            /// where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            StatusAscending,

            /// <summary>
            /// Order results by status in the following order: "Rejected", "Pending", "Inactive", "Active".
            /// Results with the same status will be sorted by full name from A to Z,
            /// where full name is a concatenation of "[First name] [Surname]".
            /// Results with the order by criteria will sorted deterministically by user ID.
            /// </summary>
            StatusDescending
        }
    }
}

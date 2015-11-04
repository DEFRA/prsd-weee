namespace EA.Weee.Core.Admin
{
    using System;
    using Shared;

    public class UserSearchData
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserStatus Status { get; set; }

        public string OrganisationName { get; set; }

        public Guid OrganisationUserId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool IsCompetentAuthorityUser { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }
}

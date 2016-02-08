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

        /// <summary>
        /// The display name of the user's role.
        /// For internal users this could be "Administrator" or "Standard".
        /// For external users this will be "N/A".
        /// </summary>
        public string Role { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }
}

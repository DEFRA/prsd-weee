namespace EA.Weee.Core.Admin
{
    using System;
    using Shared;

    public class ManageUserData
    {
        public ManageUserData()
        {
            CanManageStatus = true;
        }

        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsCompetentAuthorityUser { get; set; }

        public bool CanManageStatus { get; set; }

        public UserStatus UserStatus { get; set; }
    }
}

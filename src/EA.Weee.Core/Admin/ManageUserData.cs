namespace EA.Weee.Core.Admin
{
    using System;
    using Security;
    using Shared;

    public class ManageUserData
    {
        public ManageUserData()
        {
            CanManageRoleAndStatus = true;
        }

        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsCompetentAuthorityUser { get; set; }

        public bool CanManageRoleAndStatus { get; set; }

        public UserStatus UserStatus { get; set; }

        public Role Role { get; set; }

        public bool CanEditUser { get; set; }
    }
}

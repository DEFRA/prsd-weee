namespace EA.Weee.Core.Admin
{
    using Shared;
    using System;

    public class CompetentAuthorityUserData
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid CompetentAuthorityId { get; set; }

        public UserStatus CompetentAuthorityUserStatus { get; set; }

        public UKCompetentAuthorityData CompetentAuthorityData { get; set; }
    }
}

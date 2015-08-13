namespace EA.Weee.Domain.Admin
{
    using System;
    using Prsd.Core.Domain;

    public class CompetentAuthorityUser : Entity
    {
        public string UserId { get; private set; }

        public Guid CompetentAuthorityId { get; private set; }

        public UserStatus UserStatus { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        protected CompetentAuthorityUser()
        {
        }

        public CompetentAuthorityUser(string userId, Guid competentAuthorityId, UserStatus userStatus)
        {
            UserId = userId;
            CompetentAuthorityId = competentAuthorityId;
            UserStatus = userStatus;
        }
    }
}
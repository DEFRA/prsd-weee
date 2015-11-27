namespace EA.Weee.Domain.Admin
{
    using Prsd.Core.Domain;
    using System;

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

        public void UpdateUserStatus(UserStatus userStatus)
        {
            if (userStatus == UserStatus)
            {
                return;
            }

            if (userStatus == UserStatus.Active)
            {
                if (UserStatus != UserStatus.Inactive && UserStatus != UserStatus.Pending &&
                    UserStatus != UserStatus.Rejected)
                {
                    throw new InvalidOperationException(
                        "User status must be Inactive or Pending or Rejected to transition to Active");
                }
            }
            if (userStatus == UserStatus.Inactive)
            {
                if (UserStatus != UserStatus.Active)
                {
                    throw new InvalidOperationException("User status must be Active to transition to Inactive");
                }
            }
            if (userStatus == UserStatus.Rejected)
            {
                if (UserStatus != UserStatus.Pending)
                {
                    throw new InvalidOperationException("User status must be Pending to transition to Rejected");
                }
            }
            if (userStatus == UserStatus.Pending)
            {
                throw new InvalidOperationException("User status can not be set Pending");
            }

            UserStatus = userStatus;
        }
    }
}
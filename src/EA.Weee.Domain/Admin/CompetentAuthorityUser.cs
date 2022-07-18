﻿namespace EA.Weee.Domain.Admin
{
    using Domain.User;
    using Prsd.Core.Domain;
    using Security;
    using System;

    public class CompetentAuthorityUser : Entity
    {
        public string UserId { get; private set; }

        public Guid CompetentAuthorityId { get; private set; }

        public virtual UserStatus UserStatus { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual Role Role { get; private set; }

        public virtual Guid RoleId { get; private set; }

        public virtual User User { get; private set; }

        protected CompetentAuthorityUser()
        {
        }

        public CompetentAuthorityUser(string userId, Guid competentAuthorityId, UserStatus userStatus, Role role)
        {
            UserId = userId;
            CompetentAuthorityId = competentAuthorityId;
            UserStatus = userStatus;
            Role = role;
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

        public void UpdateRole(Role role)
        {
            Role = role;
        }
    }
}
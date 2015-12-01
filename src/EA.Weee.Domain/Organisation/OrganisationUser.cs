namespace EA.Weee.Domain.Organisation
{
    using System;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Events;

    public class OrganisationUser : Entity
    {
        public string UserId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public UserStatus UserStatus { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual User User { get; private set; }

        protected OrganisationUser()
        {
        }

        public OrganisationUser(Guid userId, Guid organisationId, UserStatus userStatus)
        {
            UserId = userId.ToString();
            OrganisationId = organisationId;
            UserStatus = userStatus;

            if (userStatus == UserStatus.Pending)
            {
                // Raise a domain event indicating that a user's request to join an organisation is pending.
                RaiseEvent(new OrganisationUserRequestEvent(this.OrganisationId, userId));
            }
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

            if (UserStatus == UserStatus.Pending)
            {
                // Raise a domain event indicating that the user's pending request has completed.
                RaiseEvent(new OrganisationUserRequestCompletedEvent(this));
            }

            UserStatus = userStatus;
        }
    }
}
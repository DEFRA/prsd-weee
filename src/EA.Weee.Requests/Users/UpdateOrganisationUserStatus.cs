namespace EA.Weee.Requests.Users
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class UpdateOrganisationUserStatus : IRequest<int>
    {
        public Guid OrganisationUserId { get; set; }

        public UserStatus UserStatus { get; set; }

        public UpdateOrganisationUserStatus(Guid organisationUserId, UserStatus userStatus)
        {
            OrganisationUserId = organisationUserId;
            UserStatus = userStatus;
        }
    }
}

namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

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

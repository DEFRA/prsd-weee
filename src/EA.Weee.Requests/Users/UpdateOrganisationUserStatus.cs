namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class UpdateOrganisationUserStatus : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public Guid OrganisationUserId { get; set; }

        public UserStatus UserStatus { get; set; }

        public UpdateOrganisationUserStatus(Guid organisationUserId, UserStatus userStatus, Guid orgId)
        {
            OrganisationUserId = organisationUserId;
            UserStatus = userStatus;
            OrganisationId = orgId;
        }
    }
}

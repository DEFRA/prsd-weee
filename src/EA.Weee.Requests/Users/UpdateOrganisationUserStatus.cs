namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class UpdateOrganisationUserStatus : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public string UserId { get; set; }

        public UserStatus UserStatus { get; set; }

        public UpdateOrganisationUserStatus(string userId, UserStatus userStatus, Guid orgId)
        {
           UserId = userId;
            UserStatus = userStatus;
            OrganisationId = orgId;
        }
    }
}

namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class UpdateCompetentAuthorityUserRoleAndStatus : IRequest<Guid>
    {
        public Guid Id { get; set; }

        public UserStatus UserStatus { get; set; }

        public string RoleName { get; set; }

        public UpdateCompetentAuthorityUserRoleAndStatus(Guid id, UserStatus userStatus, string role)
        {
            Id = id;
            UserStatus = userStatus;
            RoleName = role;
        }
    }
}

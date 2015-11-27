namespace EA.Weee.Requests.Admin
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class UpdateCompetentAuthorityUserStatus : IRequest<Guid>
    {
         public Guid Id { get; set; }

        public UserStatus UserStatus { get; set; }

        public UpdateCompetentAuthorityUserStatus(Guid id, UserStatus userStatus)
        {
            Id = id;
            UserStatus = userStatus;
        }
    }
}

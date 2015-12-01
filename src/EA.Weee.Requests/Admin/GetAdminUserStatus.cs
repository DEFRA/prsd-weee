namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetAdminUserStatus : IRequest<UserStatus>
    {
        public string UserId { get; set; }

        public GetAdminUserStatus(string userId)
        {
            UserId = userId;
        }
    }
}

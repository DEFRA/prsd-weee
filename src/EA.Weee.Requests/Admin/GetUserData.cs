namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetUserData : IRequest<ManageUserData>
    {
        public Guid OrganisationUserId { get; set; }

        public GetUserData(Guid organisationUserId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}

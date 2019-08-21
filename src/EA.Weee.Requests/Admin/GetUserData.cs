namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetUserData : IRequest<ManageUserData>
    {
        public Guid OrganisationUserId { get; set; }

        public GetUserData(Guid organisationUserId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}

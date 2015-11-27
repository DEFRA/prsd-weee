namespace EA.Weee.Requests.Users
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;
    
    public class GetOrganisationUser : IRequest<OrganisationUserData>
    {
        public Guid OrganisationUserId { get; set; }

        public GetOrganisationUser(Guid organisationUserId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}

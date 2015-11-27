namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;
    
    public class GetOrganisationUser : IRequest<OrganisationUserData>
    {
        public Guid OrganisationUserId { get; set; }

        public GetOrganisationUser(Guid organisationUserId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}

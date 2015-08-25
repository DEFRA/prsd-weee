namespace EA.Weee.Requests.Users
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;
    
    public class GetOrganisationUser : IRequest<OrganisationUserData>
    {
        public Guid OrganisationId { get; set; }

        public Guid UserId { get; set; }

        public GetOrganisationUser(Guid organisationId, Guid userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}

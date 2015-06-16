namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;

    public class GetOrganisationPrincipalPlaceOfBusiness : IRequest<OrganisationData>
    {
        public Guid OrganisationId { get; private set; }

        public GetOrganisationPrincipalPlaceOfBusiness(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class GetOrganisationInfo : IRequest<OrganisationData>
    {
        public Guid OrganisationId { get; set; }

        public GetOrganisationInfo(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;

    public class GetOrganisationInfo : IRequest<OrganisationData>
    {
        public Guid OrganisationId { get; set; }

        public GetOrganisationInfo(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

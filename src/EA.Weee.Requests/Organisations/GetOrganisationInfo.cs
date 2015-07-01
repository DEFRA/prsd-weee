namespace EA.Weee.Requests.Organisations
{
    using System;
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

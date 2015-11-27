namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using EA.Prsd.Core.Mediator;
    using System;

    public class GetOrganisationOverview : IRequest<OrganisationOverview>
    {
        public Guid OrganisationId { get; set; }

        public GetOrganisationOverview(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

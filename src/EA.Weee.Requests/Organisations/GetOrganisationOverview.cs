namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using EA.Prsd.Core.Mediator;

    public class GetOrganisationOverview : IRequest<OrganisationOverview>
    {
        public Guid OrganisationId { get; set; }

        public GetOrganisationOverview(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

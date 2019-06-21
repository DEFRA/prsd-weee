namespace EA.Weee.Requests.Admin
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;

    public class GetInternalOrganisation : IRequest<OrganisationData>
    {
        public Guid OrganisationId { get; set; }

        public GetInternalOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

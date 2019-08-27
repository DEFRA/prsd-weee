namespace EA.Weee.Requests.Admin
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;

    public class GetInternalOrganisation : IRequest<OrganisationData>
    {
        public Guid OrganisationId { get; set; }

        public GetInternalOrganisation(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}

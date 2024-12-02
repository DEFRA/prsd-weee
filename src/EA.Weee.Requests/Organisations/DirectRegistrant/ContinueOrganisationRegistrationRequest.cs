namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;

    public class ContinueOrganisationRegistrationRequest : IRequest<OrganisationTransactionData>
    {
        public Guid OrganisationId { get; private set; }

        public ContinueOrganisationRegistrationRequest(Guid organisationId)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            OrganisationId = organisationId;
        }
    }
}

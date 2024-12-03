namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using System;

    public class CompleteMigratedOrganisationTransaction : IRequest<Guid>
    {
        public Guid DirectRegistrantId { get; private set; }

        public CompleteMigratedOrganisationTransaction(Guid directRegistrantId)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);

            DirectRegistrantId = directRegistrantId;
        }
    }
}

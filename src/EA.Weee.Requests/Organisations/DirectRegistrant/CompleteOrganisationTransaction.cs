namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;

    public class CompleteOrganisationTransaction : IRequest<Guid>
    {
        public OrganisationTransactionData OrganisationTransactionData { get; private set; }

        public CompleteOrganisationTransaction(OrganisationTransactionData organisationTransactionData)
        {
            Condition.Requires(organisationTransactionData).IsNotNull();

            this.OrganisationTransactionData = organisationTransactionData;
        }
    }
}

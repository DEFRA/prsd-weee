namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using System;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;

    public class AddUpdateOrganisationTransaction : IRequest<OrganisationTransactionData>
    {
        public OrganisationTransactionData OrganisationTransactionData { get; private set; }

        public AddUpdateOrganisationTransaction(OrganisationTransactionData organisationTransactionData)
        {
            Condition.Requires(organisationTransactionData).IsNotNull();

            this.OrganisationTransactionData = organisationTransactionData;
        }
    }
}

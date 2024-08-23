namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;

    public class CompleteOrganisationTransaction : IRequest<bool>
    {
        public OrganisationTransactionData OrganisationTransactionData { get; private set; }

        public CompleteOrganisationTransaction(OrganisationTransactionData organisationTransactionData)
        {
            Condition.Requires(organisationTransactionData).IsNotNull();

            this.OrganisationTransactionData = organisationTransactionData;
        }
    }
}

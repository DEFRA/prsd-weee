using System;
using CuttingEdge.Conditions;
using EA.Prsd.Core.Mediator;
using EA.Weee.Core.Organisations;

namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    public class AddUpdateOrganisationTransaction : IRequest<Guid>
    {
        public OrganisationTransactionData OrganisationTransactionData { get; private set; }

        public AddUpdateOrganisationTransaction(OrganisationTransactionData organisationTransactionData)
        {
            Condition.Requires(organisationTransactionData).IsNotNull();

            this.OrganisationTransactionData = organisationTransactionData;
        }
    }
}

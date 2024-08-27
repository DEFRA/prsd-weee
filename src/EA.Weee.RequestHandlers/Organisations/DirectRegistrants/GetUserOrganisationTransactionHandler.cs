namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Threading.Tasks;

    internal class GetUserOrganisationTransactionHandler : IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IJsonSerializer serializer;
        private readonly IOrganisationTransactionDataAccess transactionDataAccess;

        public GetUserOrganisationTransactionHandler(IWeeeAuthorization authorization, IOrganisationTransactionDataAccess transactionDataAccess, IJsonSerializer serializer)
        {
            this.authorization = authorization;
            this.transactionDataAccess = transactionDataAccess;
            this.serializer = serializer;
        }

        public async Task<OrganisationTransactionData> HandleAsync(GetUserOrganisationTransaction query)
        {
            authorization.EnsureCanAccessExternalArea();

            var transaction = await transactionDataAccess.FindIncompleteTransactionForCurrentUserAsync();

            return transaction == null ? null : serializer.Deserialize<OrganisationTransactionData>(transaction.OrganisationJson);
        }
    }
}

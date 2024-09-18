namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.Helpers;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Threading.Tasks;

    internal class AddUpdateOrganisationTransactionHandler : IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer serializer;

        public AddUpdateOrganisationTransactionHandler(IWeeeAuthorization authorization, IOrganisationTransactionDataAccess organisationTransactionDataAccess, IJsonSerializer serializer)
        {
            this.authorization = authorization;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.serializer = serializer;
        }

        public async Task<OrganisationTransactionData> HandleAsync(AddUpdateOrganisationTransaction transaction)
        { 
            authorization.EnsureCanAccessExternalArea();

            var organisationJson = serializer.Serialize(transaction.OrganisationTransactionData);

            var organisationTransaction = await organisationTransactionDataAccess.UpdateOrCreateTransactionAsync(organisationJson);

            return serializer.Deserialize<OrganisationTransactionData>(organisationTransaction.OrganisationJson);
        }
    }
}

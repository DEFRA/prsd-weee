namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Threading.Tasks;

    internal class CompleteOrganisationTransactionHandler : IRequestHandler<CompleteOrganisationTransaction, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer jsonSerializer;

        public CompleteOrganisationTransactionHandler(
            IWeeeAuthorization authorization,
            IOrganisationTransactionDataAccess organisationTransactionDataAccess,
            IJsonSerializer jsonSerializer)
        {
            this.authorization = authorization;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<bool> HandleAsync(CompleteOrganisationTransaction request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisationJson = jsonSerializer.Serialize(request.OrganisationTransactionData);

            await organisationTransactionDataAccess.CompleteTransactionAsync(organisationJson);

            return true;
        }
    }
}
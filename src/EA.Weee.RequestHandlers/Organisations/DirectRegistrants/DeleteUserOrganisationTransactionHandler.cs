namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Threading.Tasks;

    internal class DeleteUserOrganisationTransactionHandler : IRequestHandler<DeleteUserOrganisationTransaction, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer serializer;

        public DeleteUserOrganisationTransactionHandler(IWeeeAuthorization authorization, IOrganisationTransactionDataAccess organisationTransactionDataAccess, IJsonSerializer serializer)
        {
            this.authorization = authorization;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.serializer = serializer;
        }

        public async Task<bool> HandleAsync(DeleteUserOrganisationTransaction request)
        { 
            authorization.EnsureCanAccessExternalArea();

            await organisationTransactionDataAccess.DeleteAsync();

            return true;
        }
    }
}

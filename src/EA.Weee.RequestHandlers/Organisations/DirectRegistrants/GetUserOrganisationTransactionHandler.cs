namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;

    internal class GetUserOrganisationTransactionHandler : IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;

        public GetUserOrganisationTransactionHandler(WeeeContext context, IUserContext userContext, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.userContext = userContext;
            this.authorization = authorization;
        }

        public async Task<OrganisationTransactionData> HandleAsync(GetUserOrganisationTransaction query)
        {
            authorization.EnsureCanAccessExternalArea();

            return new OrganisationTransactionData();
        }
    }
}

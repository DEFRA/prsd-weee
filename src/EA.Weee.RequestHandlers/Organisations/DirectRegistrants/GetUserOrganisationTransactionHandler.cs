namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;

    internal class GetUserOrganisationTransactionHandler : IRequestHandler<GetUserOrganisationTransaction, OrganisationTransactionData>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public GetUserOrganisationTransactionHandler(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<OrganisationTransactionData> HandleAsync(GetUserOrganisationTransaction query)
        {
            return new OrganisationTransactionData();
        }
    }
}

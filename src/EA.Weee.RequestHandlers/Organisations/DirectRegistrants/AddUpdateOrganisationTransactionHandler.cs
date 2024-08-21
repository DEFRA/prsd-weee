using EA.Weee.Core.Organisations;

namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    internal class AddUpdateOrganisationTransactionHandler : IRequestHandler<AddUpdateOrganisationTransaction, OrganisationTransactionData>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;

        public AddUpdateOrganisationTransactionHandler(WeeeContext context, IUserContext userContext, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.userContext = userContext;
            this.authorization = authorization;
        }

        public async Task<OrganisationTransactionData> HandleAsync(AddUpdateOrganisationTransaction query)
        { 
            authorization.EnsureCanAccessExternalArea();

            return new OrganisationTransactionData();
        }
    }
}

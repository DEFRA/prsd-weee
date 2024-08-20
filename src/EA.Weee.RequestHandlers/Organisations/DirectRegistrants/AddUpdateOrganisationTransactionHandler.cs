namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    internal class AddUpdateOrganisationTransactionHandler : IRequestHandler<AddUpdateOrganisationTransaction, Guid>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public AddUpdateOrganisationTransactionHandler(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(AddUpdateOrganisationTransaction query)
        {
           return Guid.NewGuid(); 
        }
    }
}

namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;
    using Security;
    using System;
    using System.Threading.Tasks;

    public class CreateSoleTraderRequestHandler : IRequestHandler<CreateSoleTraderRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext db;
        private readonly IUserContext userContext;

        public CreateSoleTraderRequestHandler(IWeeeAuthorization authorization, WeeeContext db, IUserContext userContext)
        {
            this.authorization = authorization;
            this.db = db;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CreateSoleTraderRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = Organisation.CreateSoleTrader(message.TradingName);
            db.Organisations.Add(organisation);

            await db.SaveChangesAsync();

            var organisationUser = new OrganisationUser(userContext.UserId, organisation.Id, UserStatus.Active);
            db.OrganisationUsers.Add(organisationUser);

            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;

    public class CreateSoleTraderRequestHandler : IRequestHandler<CreateSoleTraderRequest, Guid>
    {
        private readonly WeeeContext db;
        private readonly IUserContext userContext;

        public CreateSoleTraderRequestHandler(WeeeContext db, IUserContext userContext)
        {
            this.db = db;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CreateSoleTraderRequest message)
        {
            var organisation = Organisation.CreateSoleTrader(message.TradingName);
            db.Organisations.Add(organisation);

            await db.SaveChangesAsync();

            var organisationUser = new OrganisationUser(userContext.UserId, organisation.Id, UserStatus.Approved);
            db.OrganisationUsers.Add(organisationUser);

            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;

    public class CreateSoleTraderRequestHandler : IRequestHandler<CreateSoleTraderRequest, Guid>
    {
        private readonly WeeeContext db;

        public CreateSoleTraderRequestHandler(WeeeContext db)
        {
            this.db = db;
        }

        public async Task<Guid> HandleAsync(CreateSoleTraderRequest message)
        {
            var organisation = new Organisation(message.TradingName, OrganisationType.SoleTrader, OrganisationStatus.Incomplete); 
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

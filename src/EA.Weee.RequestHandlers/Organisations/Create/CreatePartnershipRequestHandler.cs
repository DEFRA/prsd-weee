namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;

    public class CreatePartnershipRequestHandler : IRequestHandler<CreatePartnershipRequest, Guid>
    {
        private readonly WeeeContext db;

        public CreatePartnershipRequestHandler(WeeeContext db)
        {
            this.db = db;
        }

        public async Task<Guid> HandleAsync(CreatePartnershipRequest message)
        {
            var organisation = new Organisation(null, message.TradingName, OrganisationType.Partnership,
                OrganisationStatus.Incomplete);
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

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
        private readonly IMap<CreatePartnershipRequest, Organisation> mapper;

        public CreatePartnershipRequestHandler(WeeeContext db, IMap<CreatePartnershipRequest, Organisation> mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Guid> HandleAsync(CreatePartnershipRequest message)
        {
            var organisation = mapper.Map(message);
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

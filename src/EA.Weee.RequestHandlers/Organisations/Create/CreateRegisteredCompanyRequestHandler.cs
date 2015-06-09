namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;

    public class CreateRegisteredCompanyRequestHandler : IRequestHandler<CreateRegisteredCompanyRequest, Guid>
    {
        private readonly WeeeContext db;
        private readonly IMap<CreateRegisteredCompanyRequest, Organisation> mapper;

        public CreateRegisteredCompanyRequestHandler(WeeeContext db, IMap<CreateRegisteredCompanyRequest, Organisation> mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Guid> HandleAsync(CreateRegisteredCompanyRequest message)
        {
            var organisation = mapper.Map(message);
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

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
        private readonly IMap<CreateSoleTraderRequest, Organisation> mapping; 

        public CreateSoleTraderRequestHandler(WeeeContext db, IMap<CreateSoleTraderRequest, Organisation> mapping)
        {
            this.db = db;
            this.mapping = mapping;
        }

        public async Task<Guid> HandleAsync(CreateSoleTraderRequest message)
        {
            var organisation = mapping.Map(message); 
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}

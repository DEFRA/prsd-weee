namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System.Threading.Tasks;

    internal class GetContactPersonByOrganisationIdHandler : IRequestHandler<GetContactPersonByOrganisationId, ContactData>
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, ContactData> mapper;

        public GetContactPersonByOrganisationIdHandler(WeeeContext context, IMap<Organisation, ContactData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ContactData> HandleAsync(GetContactPersonByOrganisationId message)
        {
            var organisation = await context.Organisations.SingleAsync(n => n.Id == message.OrganisationsId);

            return mapper.Map(organisation);
        }
    }
}

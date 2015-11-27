namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class GetContactPersonByOrganisationIdHandler : IRequestHandler<GetContactPersonByOrganisationId, ContactData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, ContactData> mapper;

        public GetContactPersonByOrganisationIdHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<Organisation, ContactData> mapper)
        {
            this.authorization = authorization;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ContactData> HandleAsync(GetContactPersonByOrganisationId message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationsId);

            var organisation = await context.Organisations.SingleAsync(n => n.Id == message.OrganisationsId);

            return mapper.Map(organisation);
        }
    }
}

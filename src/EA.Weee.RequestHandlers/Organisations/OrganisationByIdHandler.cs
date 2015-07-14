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

    internal class OrganisationByIdHandler : IRequestHandler<GetOrganisationInfo, OrganisationData>
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public OrganisationByIdHandler(WeeeContext context, IMap<Organisation, OrganisationData> organisationMap)
        {
            this.context = context;
            this.organisationMap = organisationMap;
        }

        public async Task<OrganisationData> HandleAsync(GetOrganisationInfo query)
        {
            // Need to materialize EF request before mapping (because mapping parses enums)
            var org = await context.Organisations
                .SingleAsync(o => o.Id == query.OrganisationId);

            return organisationMap.Map(org);
        }
    }
}
namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetAllOrganisationsHandler : IRequestHandler<GetAllOrganisations, List<OrganisationData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public GetAllOrganisationsHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<Organisation, OrganisationData> organisationMap)
        {
            this.authorization = authorization;
            this.context = context;
            this.organisationMap = organisationMap;
        }

        public async Task<List<OrganisationData>> HandleAsync(GetAllOrganisations message)
        {
            var orgs = await context.Organisations.ToListAsync();

            var orgsData = orgs.Select(item => organisationMap.Map(item)).ToList();

            return orgsData;
        }
    }
}
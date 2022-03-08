namespace EA.Weee.RequestHandlers.Organisations
{
    using Domain.Organisation;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAllOrganisationsHandler : IRequestHandler<GetAllOrganisations, List<OrganisationNameStatus>>
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationNameStatus> ogranisationMap;

        public GetAllOrganisationsHandler(WeeeContext context, IMap<Organisation, OrganisationNameStatus> ogranisationMap)
        {
            this.context = context;
            this.ogranisationMap = ogranisationMap;
        }

        public async Task<List<OrganisationNameStatus>> HandleAsync(GetAllOrganisations query)
        {
            var org = await context.Organisations.ToListAsync();

            if (org == null)
            {
               throw new ArgumentException("Could not find any organisations in the database.");
            }

           return org.Select(item => ogranisationMap.Map(item)).ToList();
        }
    }
}

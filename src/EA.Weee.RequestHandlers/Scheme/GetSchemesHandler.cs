namespace EA.Weee.RequestHandlers.PCS
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.PCS;
    using DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class GetSchemesHandler : IRequestHandler<GetSchemes, List<SchemeData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesHandler(WeeeContext context, IMap<Scheme, SchemeData> schemeMap)
        {
            this.context = context;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemes message)
        {
            var schemes = await context.Schemes
                .Where(s => s.Organisation.OrganisationStatus.Value == OrganisationStatus.Complete.Value)
                .ToListAsync();
             
            return schemes.Select(s => schemeMap.Map(s))
                .OrderBy(sd => sd.Name)
                .ToList();
        }
    }
}

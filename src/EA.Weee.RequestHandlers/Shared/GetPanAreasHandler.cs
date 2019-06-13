namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.Shared;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Lookup;

    public class GetPanAreasHandler : IRequestHandler<GetPanAreas, IList<PanAreaData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<PanArea, PanAreaData> mapper;

        public GetPanAreasHandler(WeeeContext context, IMap<PanArea, PanAreaData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IList<PanAreaData>> HandleAsync(GetPanAreas message)
        {
            var panareas =
                await context.PanAreas.OrderBy(c => c.Name).ToArrayAsync();

            return panareas.Select(mapper.Map).ToArray();
        }
    }
}

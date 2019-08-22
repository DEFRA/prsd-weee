﻿namespace EA.Weee.RequestHandlers.Shared
{
    using Domain.Lookup;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Shared;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetPanAreasHandler : IRequestHandler<GetPanAreas, IList<PanAreaData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<PanArea, PanAreaData> mapper;
        private readonly IWeeeAuthorization authorization;

        public GetPanAreasHandler(WeeeContext context, IWeeeAuthorization authorization, IMap<PanArea, PanAreaData> mapper)
        {
            this.context = context;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<IList<PanAreaData>> HandleAsync(GetPanAreas message)
        {
            authorization.EnsureCanAccessInternalArea();

            var panareas = await context.PanAreas.OrderBy(c => c.Name).ToArrayAsync();

            return panareas.Select(mapper.Map).ToArray();
        }
    }
}

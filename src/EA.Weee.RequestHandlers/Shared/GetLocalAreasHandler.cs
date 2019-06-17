namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mediator;
    using Requests.Admin;

    internal class GetLocalAreasHandler : IRequestHandler<GetLocalAreas, List<LocalAreaData>>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<LocalArea, LocalAreaData> mapper;

        public GetLocalAreasHandler(WeeeContext context, IWeeeAuthorization authorization, IMap<LocalArea, LocalAreaData> mapper)
        {
            this.context = context;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<List<LocalAreaData>> HandleAsync(GetLocalAreas query)
        {
            authorization.EnsureCanAccessInternalArea();

            var localareas = await context.LocalAreas.OrderBy(c => c.Name).ToListAsync();

            return localareas.Select(mapper.Map).ToList();
        }
    }
}
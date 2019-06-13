namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mediator;
    using Requests.Admin;

    internal class GetLocalAreasHandler : IRequestHandler<GetLocalAreas, List<LocalAreaData>>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public GetLocalAreasHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }

        public async Task<List<LocalAreaData>> HandleAsync(GetLocalAreas query)
        {
            authorization.EnsureCanAccessInternalArea();

            return await context.LocalAreas.OrderBy(x => x.Name)
                .Select(p => new LocalAreaData() { Id = p.Id, Name = p.Name, CompetentAuthorityId = p.CompetentAuthorityId})
                .ToListAsync();
        }
    }
}
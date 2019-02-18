namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetSchemesExternalHandler : IRequestHandler<GetSchemesExternal, List<SchemeData>>
    {
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly IWeeeAuthorization authorization;

        public GetSchemesExternalHandler(
            IGetSchemesDataAccess dataAccess,
            IMap<Scheme, SchemeData> schemeMap,
            IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
            this.authorization = authorization;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesExternal message)
        {
            authorization.EnsureCanAccessExternalArea();

            var schemes = await dataAccess.GetAllSchemesApprovedAndWithdrawn();

            return schemes.Select(s => schemeMap.Map(s))
                .OrderBy(sd => sd.Name)
                .ToList();
        }
    }
}

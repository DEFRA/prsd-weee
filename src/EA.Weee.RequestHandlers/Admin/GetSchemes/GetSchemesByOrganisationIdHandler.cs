namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;

    public class GetSchemesByOrganisationIdHandler : IRequestHandler<Requests.Admin.GetSchemesByOrganisationId, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesByOrganisationIdHandler(IWeeeAuthorization authorization, IMap<Scheme, SchemeData> schemeMap, IGetSchemesDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(Requests.Admin.GetSchemesByOrganisationId request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<Scheme> schemes = await dataAccess.GetSchemes();

            return schemes
                .Where(s => s.OrganisationId == request.OrganisationId)
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}

namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAllApprovedSchemesHandler : IRequestHandler<GetAllApprovedSchemes, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAllApprovedSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetAllApprovedSchemesHandler(IWeeeAuthorization authorization,  IMap<Scheme, SchemeData> schemeMap, IGetAllApprovedSchemesDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(GetAllApprovedSchemes request)
        {
            authorization.EnsureCanAccessInternalArea();
            var schemes = await dataAccess.GetAllApprovedSchemes();

            return schemes.Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}

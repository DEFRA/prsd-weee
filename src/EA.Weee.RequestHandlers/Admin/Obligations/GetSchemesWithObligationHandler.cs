namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Shared;
    using Weee.Security;

    public class GetSchemesWithObligationHandler : IRequestHandler<GetSchemesWithObligation, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesWithObligationHandler(IWeeeAuthorization authorization,
            IObligationDataAccess obligationDataAccess,
            IMap<Scheme, SchemeData> schemeMap)
        {
            this.authorization = authorization;
            this.obligationDataAccess = obligationDataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesWithObligation request)
        {
            authorization.EnsureCanAccessInternalArea();

            var schemesWithObligation = await obligationDataAccess.GetSchemesWithObligations(request.ComplianceYear);

            var mappedSchemes = schemesWithObligation.Select(s => schemeMap.Map(s)).ToList();

            return mappedSchemes;
        }
    }
}
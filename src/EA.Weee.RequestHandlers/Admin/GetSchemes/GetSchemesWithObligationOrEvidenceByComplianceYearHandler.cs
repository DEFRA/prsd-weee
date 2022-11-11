namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.Admin.Reports;

    public class GetSchemesWithObligationOrEvidenceByComplianceYearHandler : IRequestHandler<GetSchemesWithObligationOrEvidence, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligationDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesWithObligationOrEvidenceByComplianceYearHandler(IWeeeAuthorization authorization, IMap<Scheme, SchemeData> schemeMap, IObligationDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesWithObligationOrEvidence request)
        {
            authorization.EnsureCanAccessInternalArea();

            var schemes = await dataAccess.GetSchemesWithObligationOrEvidence(request.ComplianceYear);

            return schemes
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}

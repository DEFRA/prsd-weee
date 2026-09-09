namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using Prsd.Core.Mediator;
    using Requests;
    using Security;
    using static EA.Weee.Requests.Admin.GetSchemes;

    internal class GetSchemesForComplianceYearHandler : IRequestHandler<GetSchemesForComplianceYear, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;
        private readonly IGetSchemesForComplianceYearDataAccess dataAccess;
        private readonly IGetSchemeData getSchemeDataStub;

        public GetSchemesForComplianceYearHandler(
            IWeeeAuthorization authorization,
            IMap<Scheme, SchemeData> schemeMap,
            WeeeContext context,
            IGetSchemesForComplianceYearDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.schemeMap = schemeMap;
            this.context = context;
            this.dataAccess = dataAccess;
        }
        public GetSchemesForComplianceYearHandler(
            IWeeeAuthorization authorization,
            IMap<Scheme, SchemeData> schemeMap,
            WeeeContext context,
            IGetSchemesForComplianceYearDataAccess dataAccess,
            IGetSchemeData getSchemeDataStub)
        {
            this.authorization = authorization;
            this.schemeMap = schemeMap;
            this.context = context;
            this.dataAccess = dataAccess;
            this.getSchemeDataStub = getSchemeDataStub;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesForComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<string> schemes = await dataAccess.GetItemsAsync(request.ComplianceYear);

            List<SchemeData> schemeData = GetSchemeData();

            return schemeData
                .Where(s => schemes.Contains(s.SchemeName))
                .OrderBy(s => s.SchemeName)
                .ToList();
        }

        public List<SchemeData> GetSchemeData()
        {
            if (getSchemeDataStub != null)
            {
                return getSchemeDataStub.GetSchemeData();
            }

            return context.Schemes
                .ToList()
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}

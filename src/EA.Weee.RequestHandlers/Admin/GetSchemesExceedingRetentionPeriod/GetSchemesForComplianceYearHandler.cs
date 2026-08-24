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

        public async Task<List<SchemeData>> HandleAsync(GetSchemesForComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            Func<Scheme, bool> filter;
            switch (request.Filter)
            {
                case FilterType.Approved:
                    filter = s => s.SchemeStatus == SchemeStatus.Approved;
                    break;

                case FilterType.ApprovedOrWithdrawn:
                    filter = s => (s.SchemeStatus == SchemeStatus.Approved) || (s.SchemeStatus == SchemeStatus.Withdrawn);
                    break;

                default:
                    throw new NotSupportedException();
            }

            List<string> schemes = await dataAccess.GetItemsAsync(request.ComplianceYear);

            return context.Schemes
                .Where(t => schemes.Contains(t.SchemeName))
                .Where(filter)
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}

namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Admin;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class GetMemberRegistrationSchemesByComplianceYearHandler : IRequestHandler<GetMemberRegistrationSchemesByComplianceYear, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetMemberRegistrationSchemesByComplianceYearHandler(IWeeeAuthorization authorization, IMap<Scheme, SchemeData> schemeMap, WeeeContext context, ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.schemeMap = schemeMap;
            this.context = context;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<List<SchemeData>> HandleAsync(GetMemberRegistrationSchemesByComplianceYear request)
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

            List<string> schemes = await schemeDataAccess.GetMemberRegistrationSchemesByComplianceYear(request.ComplianceYear);

            return context.Schemes
                .Where(t => schemes.Contains(t.SchemeName))
                .Where(filter)
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}
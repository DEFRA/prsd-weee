namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Admin;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using static EA.Weee.Requests.Admin.GetMemberRegisteredSchemesByComplianceYear;

    public class GetMemberRegistrationSchemesByComplianceYear : IRequestHandler<GetMemberRegisteredSchemesByComplianceYear, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;

        public GetMemberRegistrationSchemesByComplianceYear(IWeeeAuthorization authorization, IMap<Scheme, SchemeData> schemeMap, WeeeContext context)
        {
            this.authorization = authorization;
            this.schemeMap = schemeMap;
            this.context = context;
        }

        public async Task<List<SchemeData>> HandleAsync(GetMemberRegisteredSchemesByComplianceYear request)
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

            List<string> schemes = await context.MemberUploads
                .Where(mu => mu.IsSubmitted)
                .Where(mu => mu.ComplianceYear.HasValue && mu.ComplianceYear.Value == request.ComplianceYear)
                .Select(mu => (string)mu.Scheme.SchemeName)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync();

            return context.Schemes
                .Where(t => schemes.Contains(t.SchemeName))
                .Where(filter)
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}
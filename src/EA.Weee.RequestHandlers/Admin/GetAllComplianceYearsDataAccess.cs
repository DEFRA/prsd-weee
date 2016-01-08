namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;

    public class GetAllComplianceYearsDataAccess : IGetAllComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetAllComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> GetAllComplianceYears(ComplianceYearFor complianceYearFor)
        {
            var complianceYears = new List<int>();

            if (complianceYearFor == ComplianceYearFor.MemberRegistrations)
            {
                complianceYears = await context.MemberUploads
                    .Where(mu => mu.IsSubmitted)
                    .Where(mu => mu.ComplianceYear.HasValue)
                    .Select(mu => (int)mu.ComplianceYear)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
            }
            if (complianceYearFor == ComplianceYearFor.DataReturns)
            {
                complianceYears = await context.DataReturnsUploads
                    .Where(dru => dru.DataReturnVersion != null)
                    .Where(dru => dru.ComplianceYear.HasValue)
                    .Select(dru => (int)dru.ComplianceYear)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
            }
            return complianceYears;
        }
    }
}

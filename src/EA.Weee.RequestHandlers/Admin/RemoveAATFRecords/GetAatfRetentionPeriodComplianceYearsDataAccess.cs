namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Weee.DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetAatfRetentionPeriodComplianceYearsDataAccess : IGetAatfRetentionPeriodComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfRetentionPeriodComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> GetAatfAeRetentionPeriodComplianceYears(int retentionPeriod)
        {
            return await context.Aatfs
                    .Select(r => (int)r.ComplianceYear - retentionPeriod)
                    .Distinct()
                    .OrderBy(year => year)
                    .ToListAsync();
        }
    }
}

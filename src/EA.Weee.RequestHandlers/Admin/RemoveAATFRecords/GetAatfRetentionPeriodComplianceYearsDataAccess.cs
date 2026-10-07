namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Weee.DataAccess;
    using System;
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
            var currentYear = DateTime.UtcNow.Year;

            return await context.Aatfs
                .Where(r => (int)r.ComplianceYear <= currentYear - retentionPeriod)
                .Select(r => (int)r.ComplianceYear)
                .Distinct()
                .OrderBy(year => year)
                .ToListAsync();
        }
    }
}

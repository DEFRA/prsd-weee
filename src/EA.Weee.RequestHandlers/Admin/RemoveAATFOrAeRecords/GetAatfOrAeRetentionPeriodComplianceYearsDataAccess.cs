namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetAatfOrAeRetentionPeriodComplianceYearsDataAccess : IGetAatfOrAeRetentionPeriodComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfOrAeRetentionPeriodComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> GetAatfAeRetentionPeriodComplianceYears(FacilityType faclityType, int retentionPeriod)
        {
            var currentYear = DateTime.UtcNow.Year;
            int facilityTypeVal = (faclityType == FacilityType.Aatf) ? 1 : 2;

            return await context.Aatfs
                .Where(r => (int)r.ComplianceYear <= currentYear - retentionPeriod && (r.FacilityType.Value == facilityTypeVal))
                .Select(r => (int)r.ComplianceYear)
                .Distinct()
                .OrderBy(year => year)
                .ToListAsync();
        }
    }
}

namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;

    public class GetDataReturnsActiveComplianceYearsDataAccess : IGetDataReturnsActiveComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnsActiveComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> Get()
        {
            return await context.DataReturnsUploads
                    .Where(dru => dru.DataReturnVersion != null)
                    .Where(dru => dru.ComplianceYear.HasValue)
                    .Select(dru => (int)dru.ComplianceYear)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
        }
    }
}

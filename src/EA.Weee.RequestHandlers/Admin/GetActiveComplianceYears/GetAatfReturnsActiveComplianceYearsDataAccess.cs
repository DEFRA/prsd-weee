namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;

    public class GetAatfReturnsActiveComplianceYearsDataAccess : IGetAatfReturnsActiveComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfReturnsActiveComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> Get()
        {
            return await context.Returns
                    .Where(r => r.SubmittedDate.HasValue)
                    .Select(r => r.Quarter.Year)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
        }
    }
}

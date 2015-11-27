namespace EA.Weee.RequestHandlers.Admin
{
    using DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAllComplianceYearsDataAccess : IGetAllComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetAllComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> GetAllComplianceYears()
        {
            return await context.MemberUploads
                 .Where(mu => mu.IsSubmitted)
                 .Where(mu => mu.ComplianceYear.HasValue)
                 .Select(mu => (int)mu.ComplianceYear)
                 .Distinct()
                 .OrderByDescending(year => year)
                 .ToListAsync();
        }
    }
}

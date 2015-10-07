namespace EA.Weee.RequestHandlers.Admin.Submissions
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;

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
                 .GroupBy(mu => (int)mu.ComplianceYear)
                 .Select(group => group.Key)
                 .OrderByDescending(year => year)
                 .ToListAsync();
        }
    }
}

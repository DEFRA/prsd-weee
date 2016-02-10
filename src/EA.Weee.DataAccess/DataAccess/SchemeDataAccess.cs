namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public class SchemeDataAccess : ISchemeDataAccess
    {
        private readonly WeeeContext context;

        public SchemeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Scheme> GetSchemeOrDefault(Guid schemeId)
        {
            return await context.Schemes.FindAsync(schemeId);
        }

        public async Task<IList<int>> GetComplianceYearsWithSubmittedMemberUploads(Guid schemeId)
        {
            return await context.MemberUploads
                .Where(mu => mu.Scheme != null)
                .Where(mu => mu.Scheme.Id == schemeId)
                .Where(mu => mu.ComplianceYear.HasValue)
                .Where(mu => mu.IsSubmitted)
                .Select(mu => mu.ComplianceYear.Value)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IList<int>> GetComplianceYearsWithSubmittedDataReturns(Guid schemeId)
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme != null)
                .Where(dr => dr.Scheme.Id == schemeId)
                .Where(dr => dr.CurrentVersion != null)
                .Where(dr => dr.CurrentVersion.SubmittedDate.HasValue)
                .Select(dr => dr.Quarter.Year)
                .Distinct()
                .ToListAsync();
        }
    }
}

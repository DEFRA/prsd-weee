namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task<Scheme> GetSchemeOrDefaultByApprovalNumber(string approvalNumber)
        {
            return await context.Schemes.Where(s => s.ApprovalNumber == approvalNumber).FirstOrDefaultAsync();
        }

        public async Task<Scheme> GetSchemeOrDefaultByOrganisationId(Guid organisationId)
        {
            return await context.Schemes.FirstOrDefaultAsync(s => s.OrganisationId == organisationId);
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

        public async Task<List<string>> GetMemberRegistrationSchemesByComplianceYear(int complianceYear)
        {
            return await context.MemberUploads
                .Where(mu => mu.IsSubmitted)
                .Where(mu => mu.ComplianceYear.HasValue && mu.ComplianceYear.Value == complianceYear)
                .Select(mu => (string)mu.Scheme.SchemeName)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync();
        }

        public async Task<List<string>> GetEEEWEEEDataReturnSchemesByComplianceYear(int complianceYear)
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme != null)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .Where(dr => dr.CurrentVersion != null)
                .Where(dr => dr.CurrentVersion.SubmittedDate.HasValue)
                .Select(dr => (string)dr.Scheme.SchemeName)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync();
        }
    }
}

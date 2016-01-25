namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnComplianceYearsForScheme
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;

    public class FetchDataReturnComplianceYearsForSchemeDataAccess : IFetchDataReturnComplianceYearsForSchemeDataAccess
    {
        private readonly WeeeContext context;

        public FetchDataReturnComplianceYearsForSchemeDataAccess(WeeeContext dbContext)
        {
            this.context = dbContext;
        }

        public async Task<List<int>> GetDataReturnComplianceYearsForScheme(Guid schemeId)
        {
            return await context.DataReturnsUploads
                    .Where(dru => dru.Scheme.Id == schemeId)
                    .Where(dru => dru.DataReturnVersion != null)
                    .Where(dru => dru.ComplianceYear.HasValue)
                    .Select(dru => (int)dru.ComplianceYear)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
        }

        public async Task<Domain.Scheme.Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Schemes.SingleAsync(c => c.OrganisationId == organisationId);
        }
    }
}

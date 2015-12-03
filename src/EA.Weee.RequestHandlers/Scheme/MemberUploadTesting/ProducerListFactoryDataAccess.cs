namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;

    public class ProducerListFactoryDataAccess : IProducerListFactoryDataAccess
    {
        private readonly WeeeContext context;

        public ProducerListFactoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SchemeInfo>> FetchSchemeInfo(Guid organisationId)
        {
            var query = await context.Schemes
                .Where(s => s.OrganisationId == organisationId)
                .Select(s => new { TradingName = s.Organisation.TradingName, ApprovalNumber = s.ApprovalNumber })
                .ToListAsync();

            return query
                .Select(x => new SchemeInfo { TradingName = x.TradingName, ApprovalNumber = x.ApprovalNumber })
                .ToList();
        }

        public Task<List<string>> GetRegistrationNumbers(Guid organisationId, int complianceYear, int numberOfRegistrationNumberToInclude)
        {
            return context.RegisteredProducers
                .Where(p => p.CurrentSubmission != null)
                .Where(p => p.ComplianceYear == complianceYear)
                .Where(p => p.Scheme.OrganisationId == organisationId)
                .Select(p => p.ProducerRegistrationNumber)
                .Take(numberOfRegistrationNumberToInclude)
                .ToListAsync();
        }
    }
}

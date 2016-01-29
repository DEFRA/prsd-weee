namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain.DataReturns;

    public class FetchSummaryCsvDataAccess : IFetchSummaryCsvDataAccess
    {
        private readonly WeeeContext context;

        public FetchSummaryCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Scheme.Scheme> FetchSchemeAsync(Guid organisationId)
        {
            return await context.Schemes
                .Where(s => s.Organisation.Id == organisationId)
                .SingleAsync();
        }

        public async Task<DataReturn> FetchDataReturnOrDefaultAsync(Guid organisationId, int complianceYear, QuarterType quarterType)
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme.Organisation.Id == organisationId)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .Where(dr => dr.Quarter.Q == quarterType)
                .SingleOrDefaultAsync();
        }
    }
}

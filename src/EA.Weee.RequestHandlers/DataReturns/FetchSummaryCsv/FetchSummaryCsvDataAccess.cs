namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;
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

        /// <summary>
        /// Fetches the aggregated data from the database.
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        public async Task<List<DataReturnSummaryCsvData>> FetchResultsAsync(Guid schemeId, int complianceYear)
        {
            return await context.StoredProcedures.SpgDataReturnSummaryCsv(
                schemeId,
                complianceYear);
        }
    }
}

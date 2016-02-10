namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using Domain.DataReturns;
    using Domain.Scheme;

    public interface IFetchSummaryCsvDataAccess
    {
        /// <summary>
        /// Returns the scheme for the specified organisation.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        Task<Scheme> FetchSchemeAsync(Guid organisationId);

        /// <summary>
        /// Fetches the aggregated data from the database.
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        Task<List<DataReturnSummaryCsvData>> FetchResultsAsync(Guid schemeId, int complianceYear);
    }
}

namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        /// Returns the data return for the specified organisation, compliance year and quarter if it exists.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="complianceYear"></param>
        /// <param name="quarterType"></param>
        /// <returns></returns>
        Task<DataReturn> FetchDataReturnOrDefaultAsync(Guid organisationId, int complianceYear, QuarterType quarterType);
    }
}

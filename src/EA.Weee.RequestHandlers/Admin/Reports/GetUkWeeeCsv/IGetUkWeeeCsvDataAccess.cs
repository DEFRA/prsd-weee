namespace EA.Weee.RequestHandlers.Admin.Reports.GetUKWeeeCsv
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.DataReturns;

    public interface IGetUkWeeeCsvDataAccess
    {
        /// <summary>
        /// Returns all data returns for the specified compliance year.
        /// Each data return will be pre-loaded with the current version's
        /// set of collected amounts and delivered amounts.
        /// The results will not be ordered.
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        Task<IEnumerable<DataReturn>> FetchDataReturnsForComplianceYearAsync(int complianceYear);
    }
}

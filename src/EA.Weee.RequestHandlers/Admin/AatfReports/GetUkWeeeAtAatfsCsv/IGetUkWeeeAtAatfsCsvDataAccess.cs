namespace EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetUkWeeeAtAatfsCsvDataAccess
    {
        /// <summary>
        /// Returns all data returns for the specified compliance year.
        /// Each data return will be pre-loaded with the current version's
        /// set of collected amounts and delivered amounts.
        /// The results will not be ordered.
        /// </summary>
        Task<IEnumerable<PartialAatfReturn>> FetchPartialAatfReturnsForComplianceYearAsync(int complianceYear);
    }
}

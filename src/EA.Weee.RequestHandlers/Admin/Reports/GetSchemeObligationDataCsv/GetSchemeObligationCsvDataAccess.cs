namespace EA.Weee.RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;

    public class GetSchemeObligationCsvDataAccess : IGetSchemeObligationCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeObligationCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns the list of SchemeObligationCsvData for all registered producers that have obligation data for the selected year,
        /// but with B2B records removed for producers that are also registered for B2C with a second scheme in the selected year to
        /// avoid double-counting their obligation.
        /// </summary>
        /// <param name="complianceYear">The selected compliance year to return obligation data for</param>
        /// <returns>Scheme Obligation Csv Data</returns>
        public async Task<List<SchemeObligationCsvData>> FetchObligationsForComplianceYearAsync(int complianceYear)
        {
            var allCurrentObligations = await context.StoredProcedures.SpgSchemeObligationDataCsv(complianceYear);

            IEnumerable<string> doubleSchemePRNs = allCurrentObligations.GroupBy(p => p.PRN).Where(g => g.Count() > 1).Select(g => g.Key);

            if (!doubleSchemePRNs.Any())
            {
                return allCurrentObligations;
            }
            else
            {
                return allCurrentObligations.Where(p => !(p.ObligationTypeForSelectedYear == "B2B" && doubleSchemePRNs.Contains(p.PRN))).ToList();
            }
        }
    }
}
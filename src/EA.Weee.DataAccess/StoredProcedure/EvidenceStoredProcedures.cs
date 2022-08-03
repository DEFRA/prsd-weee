namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class EvidenceStoredProcedures : IEvidenceStoredProcedures
    {
        private readonly WeeeContext context;

        public EvidenceStoredProcedures(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, int complianceYear)
        {
            string queryString = "[Evidence].[getAatfEvidenceSummaryTotals] @AatfId, @ComplianceYear";
            SqlParameter aatfIdParameter = new SqlParameter("@AatfId", aatfId);
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);

            return await context.Database.SqlQuery<AatfEvidenceSummaryTotalsData>(queryString, aatfIdParameter, complianceYearParameter).ToListAsync();
        }

        public async Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid pcsId, int complianceYear)
        {
            string queryString = "[Evidence].[getObligationEvidenceSummaryTotals] @SchemeId, @ComplianceYear";
            SqlParameter pcsIdParameter = new SqlParameter("@SchemeId", pcsId);
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);

            return await context.Database.SqlQuery<ObligationEvidenceSummaryTotalsData>(queryString, pcsIdParameter, complianceYearParameter).ToListAsync();
        }
    }
}

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

        public async Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, short complianceYear)
        {
            string queryString = "[Evidence].[getAatfEvidenceSummaryTotals] @AatfId, @ComplianceYear";
            SqlParameter aatfIdParameter = new SqlParameter("@AatfId", aatfId);
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database.SqlQuery<AatfEvidenceSummaryTotalsData>(queryString, aatfIdParameter, complianceYearParameter).ToListAsync();
        }
    }
}

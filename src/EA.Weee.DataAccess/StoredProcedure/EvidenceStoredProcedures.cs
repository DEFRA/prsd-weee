namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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

        public async Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? pcsId, Guid? orgId, int complianceYear)
        {
            string queryString = "[Evidence].[getObligationEvidenceSummaryTotals] @ComplianceYear, @OrganisationId, @SchemeId ";

            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            SqlParameter orgIdParameter = new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier);
            orgIdParameter.IsNullable = true;
            orgIdParameter.Value = orgId.HasValue ? orgId.Value : (object)DBNull.Value;
            SqlParameter pcsIdParameter = new SqlParameter("@SchemeId", SqlDbType.UniqueIdentifier);
            pcsIdParameter.IsNullable = true;
            pcsIdParameter.Value = pcsId.HasValue ? pcsId.Value : (object)DBNull.Value;

            return await context.Database.SqlQuery<ObligationEvidenceSummaryTotalsData>(queryString, complianceYearParameter, orgIdParameter, pcsIdParameter).ToListAsync();
        }
    }
}

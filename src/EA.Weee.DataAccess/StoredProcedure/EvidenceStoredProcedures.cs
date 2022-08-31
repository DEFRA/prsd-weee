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

        public async Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? pcsId, Guid orgId, int complianceYear)
        {
            string queryString = "[Evidence].[getObligationEvidenceSummaryTotals] @SchemeId, @OrganisationId, @ComplianceYear";
            SqlParameter pcsIdParameter = new SqlParameter("@SchemeId", pcsId); 
            pcsIdParameter.SqlDbType = SqlDbType.UniqueIdentifier;
            SqlParameter orgIdParameter = new SqlParameter("@OrganisationId", orgId); 
            orgIdParameter.SqlDbType = SqlDbType.UniqueIdentifier;
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);

            return await context.Database.SqlQuery<ObligationEvidenceSummaryTotalsData>(queryString, pcsIdParameter, orgIdParameter, complianceYearParameter).ToListAsync();
        }
    }
}

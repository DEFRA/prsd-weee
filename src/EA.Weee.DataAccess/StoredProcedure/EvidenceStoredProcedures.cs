namespace EA.Weee.DataAccess.StoredProcedure
{
    using CuttingEdge.Conditions;
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

        public async Task<List<EvidenceNoteReportData>> GetEvidenceNoteOriginalTonnagesReport(
            int complianceYear, Guid? originatingOrganisationId, Guid? recipientOrganisationId)
        {
            var queryString = "[Evidence].[getEvidenceNotesOriginalTonnage] @ComplianceYear, @OriginatingOrganisationId, @RecipientOrganisationId";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var orgIdParameter = new SqlParameter("@OriginatingOrganisationId", SqlDbType.UniqueIdentifier)
                {
                    IsNullable = true,
                    Value = originatingOrganisationId ?? (object)DBNull.Value
                };
            var pcsIdParameter = new SqlParameter("@RecipientOrganisationId", SqlDbType.UniqueIdentifier)
                {
                    IsNullable = true,
                    Value = recipientOrganisationId ?? (object)DBNull.Value
                };

            return await context.Database.SqlQuery<EvidenceNoteReportData>(queryString, complianceYearParameter, orgIdParameter, pcsIdParameter).ToListAsync();
        }

        public async Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, int complianceYear)
        {
            var queryString = "[Evidence].[getAatfEvidenceSummaryTotals] @AatfId, @ComplianceYear";
            var aatfIdParameter = new SqlParameter("@AatfId", aatfId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);

            return await context.Database.SqlQuery<AatfEvidenceSummaryTotalsData>(queryString, aatfIdParameter, complianceYearParameter).ToListAsync();
        }

        public async Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? pcsId, Guid? orgId, int complianceYear)
        {
            Condition.Requires(pcsId == null && orgId == null).IsFalse("pcsId and orgId cannot be both null");
            
            var queryString = "[Evidence].[getObligationEvidenceSummaryTotals] @ComplianceYear, @OrganisationId, @SchemeId ";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var orgIdParameter = new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier)
 {
     IsNullable = true,
     Value = orgId ?? (object)DBNull.Value
 };
            var pcsIdParameter = new SqlParameter("@SchemeId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = pcsId ?? (object)DBNull.Value
            };

            return await context.Database.SqlQuery<ObligationEvidenceSummaryTotalsData>(queryString, complianceYearParameter, orgIdParameter, pcsIdParameter).ToListAsync();
        }
    }
}

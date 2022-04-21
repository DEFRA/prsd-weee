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
            var aatfIdParameter = new SqlParameter("@AatfId", aatfId);

            return await context.Database.SqlQuery<AatfEvidenceSummaryTotalsData>("[Evidence].[getAatfEvidenceSummaryTotals] @AatfId", aatfIdParameter).ToListAsync();
        }
    }
}

namespace EA.Weee.DataAccess.StoredProcedure
{
    /// <summary>
    /// Maps to [Evidence].[getSchemeObligationAndEvidenceTotals]
    /// </summary>
    public class ObligationAndEvidenceProgressSummaryData
    {
        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public string CategoryName { get; set; }

        public decimal? Obligation { get; set; }

        public decimal? Evidence { get; set; }

        public decimal? Reuse { get; set; }

        public decimal? NonHouseholdEvidence { get; set; }

        public decimal? NonHouseHoldEvidenceReuse { get; set; }

        public decimal? TransferredOut { get; set; }

        public decimal? TransferredIn { get; set; }

        public decimal? ObligationDifference { get; set; }
    }
}

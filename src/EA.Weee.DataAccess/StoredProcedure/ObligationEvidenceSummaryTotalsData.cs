namespace EA.Weee.DataAccess.StoredProcedure
{
    using EA.Weee.Domain.Lookup;

    public class ObligationEvidenceSummaryTotalsData
    {
        public WeeeCategory CategoryId { get; set; }
        public decimal? Obligation { get; set; }
        public decimal? Evidence { get; set; }
        public decimal? Reuse { get; set; }
        public decimal? TransferredOut { get; set; }
        public decimal? TransferredIn { get; set; }
        public decimal? ObligationDifference { get; set; }
    }
}

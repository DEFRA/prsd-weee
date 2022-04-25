namespace EA.Weee.DataAccess.StoredProcedure
{
    using Domain.Lookup;

    public class AatfEvidenceSummaryTotalsData
    {
        public decimal? Received { get; set; }

        public decimal? Reused { get; set; }

        public WeeeCategory CategoryId { get; set; }
    }
}

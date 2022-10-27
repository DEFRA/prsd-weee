namespace EA.Weee.DataAccess.StoredProcedure
{
    using Domain.Lookup;

    public class AatfEvidenceSummaryTotalsData
    {
        public decimal? ApprovedReceived { get; set; }

        public decimal? ApprovedReused { get; set; }

        public decimal? SubmittedReceived { get; set; }

        public decimal? SubmittedReused { get; set; }

        public decimal? DraftReceived { get; set; }

        public decimal? DraftReused { get; set; }

        public WeeeCategory CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}

namespace EA.Weee.DataAccess.StoredProcedure
{
    public class UkNonObligatedWeeeReceivedData
    {
        public int Quarter { get; set; }
        public string Category { get; set; }
        public decimal? TotalNonObligatedWeeeReceived { get; set; }
        public decimal? TotalNonObligatedWeeeReceivedFromDcf { get; set; }
    }
}

namespace EA.Weee.DataAccess.StoredProcedure
{
    public class UkNonObligatedWeeeReceivedData
    {
        public string Quarter { get; set; }
        public string Category { get; set; }
        public decimal? TotalNonObligatedWeeeReceived { get; set; }
        public decimal? TotalNonObligatedWeeeReceivedFromDcf { get; set; }
    }
}

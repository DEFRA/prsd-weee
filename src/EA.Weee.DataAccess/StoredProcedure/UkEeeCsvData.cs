namespace EA.Weee.DataAccess.StoredProcedure
{
    /// <summary>
    /// This class maps to the results of [Producer].SpgUKEEEDataByComplianceYear sp.
    /// </summary>
    public class UkEeeCsvData
    {
        public string Category { get; set; }
        public decimal? TotalB2BEEE { get; set; }
        public decimal? Q1B2BEEE { get; set; }
        public decimal? Q2B2BEEE { get; set; }
        public decimal? Q3B2BEEE { get; set; }
        public decimal? Q4B2BEEE { get; set; }
        public decimal? TotalB2CEEE { get; set; }
        public decimal? Q1B2CEEE { get; set; }
        public decimal? Q2B2CEEE { get; set; }
        public decimal? Q3B2CEEE { get; set; }
        public decimal? Q4B2CEEE { get; set; }
    }
}

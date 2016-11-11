namespace EA.Weee.DataAccess.StoredProcedure
{
    /// <summary>
    /// This class maps to the results of [PCS].[spgSchemeObligationDataCsv]
    /// </summary>
    public class SchemeObligationCsvData
    {
        public string SchemeName { get; set; }
        public string ApprovalNumber { get; set; }
        public string PRN { get; set; }
        public string ProducerName { get; set; }
        public string ObligationTypeForPreviousYear { get; set; }
        public string ObligationTypeForSelectedYear { get; set; }
        public decimal? Cat1B2CTotal { get; set; }
        public decimal? Cat2B2CTotal { get; set; }
        public decimal? Cat3B2CTotal { get; set; }
        public decimal? Cat4B2CTotal { get; set; }
        public decimal? Cat5B2CTotal { get; set; }
        public decimal? Cat6B2CTotal { get; set; }
        public decimal? Cat7B2CTotal { get; set; }
        public decimal? Cat8B2CTotal { get; set; }
        public decimal? Cat9B2CTotal { get; set; }
        public decimal? Cat10B2CTotal { get; set; }
        public decimal? Cat11B2CTotal { get; set; }
        public decimal? Cat12B2CTotal { get; set; }
        public decimal? Cat13B2CTotal { get; set; }
        public decimal? Cat14B2CTotal { get; set; }
    }
}

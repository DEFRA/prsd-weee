namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [Producer].[spgProducerEeeHistoryCsvData].
    /// </summary>
    public class ProducerEeeHistoryCsvData
        {
            public string SchemeName { get; set; }
            public string ApprovalNumber { get; set; }
            public int ComplianceYear { get; set; }
            public DateTime SubmittedDate { get; set; }
            public int Quarter { get; set; }
            public string LatestData { get; set; }
            public decimal? Cat1B2C { get; set; }
            public decimal? Cat2B2C { get; set; }
            public decimal? Cat3B2C { get; set; }
            public decimal? Cat4B2C { get; set; }
            public decimal? Cat5B2C { get; set; }
            public decimal? Cat6B2C { get; set; }
            public decimal? Cat7B2C { get; set; }
            public decimal? Cat8B2C { get; set; }
            public decimal? Cat9B2C { get; set; }
            public decimal? Cat10B2C { get; set; }
            public decimal? Cat11B2C { get; set; }
            public decimal? Cat12B2C { get; set; }
            public decimal? Cat13B2C { get; set; }
            public decimal? Cat14B2C { get; set; }
            public decimal? Cat1B2B { get; set; }
            public decimal? Cat2B2B { get; set; }
            public decimal? Cat3B2B { get; set; }
            public decimal? Cat4B2B{ get; set; }
            public decimal? Cat5B2B { get; set; }
            public decimal? Cat6B2B { get; set; }
            public decimal? Cat7B2B { get; set; }
            public decimal? Cat8B2B { get; set; }
            public decimal? Cat9B2B { get; set; }
            public decimal? Cat10B2B { get; set; }
            public decimal? Cat11B2B { get; set; }
            public decimal? Cat12B2B { get; set; }
            public decimal? Cat13B2B { get; set; }
            public decimal? Cat14B2B { get; set; }
    }
    }
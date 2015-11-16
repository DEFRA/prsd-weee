namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [Producer].[spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority].
    /// </summary>
    public class PCSChargesCSVData
    {
        public string SchemeName { get; set; }

        public int ComplianceYear { get; set; }

        public string ProducerName { get; set; }
        
        public string PRN { get; set; }

        public DateTime SubmissionDate { get; set; }

        public decimal ChargeValue{ get; set; }

        public string ChargeBandType { get; set; }
    }
}

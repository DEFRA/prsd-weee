namespace EA.Weee.DataAccess.StoredProcedure
{
    public class PcsAatfComparisonDataCsvData
    {
        public int ComplianceYear { get; set; }

        public string QuarterValue { get; set; }

        public string ObligationType { get; set; }

        public string Category { get; set; }

        public string SchemeNameValue { get; set; }

        public string PcsApprovalNumber { get; set; }

        public string PcsAbbreviation { get; set; }

        public string AatfName { get; set; }

        public string AatfApprovalNumber { get; set; }

        public string AatfAbbreviation { get; set; }

        public decimal PcsTonnage { get; set; }

        public decimal AatfTonnage { get; set; }

        public decimal DifferenceTonnage { get; set; }
    }
}

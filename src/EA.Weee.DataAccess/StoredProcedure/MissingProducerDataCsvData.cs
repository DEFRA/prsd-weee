namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [Producer].[spgMissingProducerDataCsvData]
    /// </summary>
    public class MissingProducerDataCsvData
    {
        public string SchemeName { get; set; }
        public string ApprovalNumber { get; set; }
        public string ProducerName { get; set; }
        public string PRN { get; set; }
        public string ObligationType { get; set; }
        public int? Quarter { get; set; }
        public DateTime DateRegistered { get; set; }

        public MissingProducerDataCsvData Copy()
        {
            MissingProducerDataCsvData copy = new MissingProducerDataCsvData();
            copy.SchemeName = SchemeName;
            copy.ApprovalNumber = ApprovalNumber;
            copy.ProducerName = ProducerName;
            copy.PRN = PRN;
            copy.ObligationType = ObligationType;
            copy.Quarter = Quarter;
            copy.DateRegistered = DateRegistered;
            return copy;
        }
    }
}

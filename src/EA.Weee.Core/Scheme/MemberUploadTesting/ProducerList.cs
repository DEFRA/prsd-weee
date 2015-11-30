namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System.Collections.Generic;

    public class ProducerList
    {
        public SchemaVersion SchemaVersion { get; set; }
        public string ApprovalNumber { get; set; }
        public int ComplianceYear { get; set; }
        public string TradingName { get; set; }
        public SchemeBusiness SchemeBusiness { get; set; }
        public List<Producer> Producers { get; set; }

        public ProducerList()
        {
            SchemeBusiness = new SchemeBusiness();
            Producers = new List<Producer>();
        }
    }
}

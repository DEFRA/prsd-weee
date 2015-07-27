using EA.Prsd.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
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

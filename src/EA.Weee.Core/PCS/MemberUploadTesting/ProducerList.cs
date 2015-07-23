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
        public SchemeBusiness SchemeBusiness { get; private set; }
        public List<Producer> Producers { get; private set; }

        public ProducerList()
        {
            SchemeBusiness = new SchemeBusiness();
            Producers = new List<Producer>();
        }

        public static ProducerList Create(ProducerListSettings listSettings)
        {
            Guard.ArgumentNotNull(() => listSettings, listSettings);

            ProducerList producerList = new ProducerList();

            producerList.SchemaVersion = listSettings.SchemaVersion;
            producerList.ApprovalNumber = "WEE/ZZ9999ZZ/SCH";
            producerList.ComplianceYear = 2016;
            producerList.TradingName = RandomHelper.CreateRandomString(string.Empty, 1, 70);
            producerList.SchemeBusiness = SchemeBusiness.Create(listSettings);

            for (int index = 0; index < listSettings.NumberOfNewProducers; ++index)
            {
                ProducerSettings producerSettings = new ProducerSettings(listSettings.SchemaVersion);

                Producer producer = Producer.Create(producerSettings);

                producerList.Producers.Add(producer);
            }

            return producerList;
        }
    }
}

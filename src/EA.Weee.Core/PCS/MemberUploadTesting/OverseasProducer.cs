using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class OverseasProducer
    {
        public string OverseasProducerName { get; set; }
        public ContactDetails ContactDetails { get; set; }

        public OverseasProducer()
        {

        }

        internal static OverseasProducer Create(IOverseasProducerSettings settings)
        {
            OverseasProducer overseasProducer = new OverseasProducer();

            overseasProducer.OverseasProducerName = RandomHelper.CreateRandomString(string.Empty, 0, 50); // 70?

            if (RandomHelper.OneIn(2))
            {
                overseasProducer.ContactDetails = ContactDetails.Create(settings);
            }

            return overseasProducer;
        }
    }
}

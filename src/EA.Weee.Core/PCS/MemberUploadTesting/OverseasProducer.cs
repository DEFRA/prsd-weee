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

            if (!settings.IgnoreStringLengthConditions)
            {
                overseasProducer.OverseasProducerName = RandomHelper.CreateRandomString(string.Empty, 0, 50); // 70?
            }
            else
            {
                overseasProducer.OverseasProducerName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
            }

            if (RandomHelper.OneIn(2))
            {
                overseasProducer.ContactDetails = ContactDetails.Create(settings);
            }

            return overseasProducer;
        }
    }
}

namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    public class ProducerNameRegisteredBefore
    {
        public producerType Producer { get; set; }

        public string ComplianceYear { get; set; }

        public ProducerNameRegisteredBefore(producerType producer, string complianceYear)
        {
            Producer = producer;
            ComplianceYear = complianceYear;
        }
    }
}

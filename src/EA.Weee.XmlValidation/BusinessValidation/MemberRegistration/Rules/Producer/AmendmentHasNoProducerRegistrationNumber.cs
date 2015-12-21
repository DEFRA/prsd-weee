namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberRegistration;

    public class AmendmentHasNoProducerRegistrationNumber : IAmendmentHasNoProducerRegistrationNumber
    {
        public RuleResult Evaluate(producerType producer)
        {
            if (producer.status == statusType.A && string.IsNullOrEmpty(producer.registrationNo))
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "You have not provided a producer registration number (PRN) for {0} when attempting to amend existing producer details. To amend this producer add the PRN or to add as a brand new producer use status 'I' in the XML.",
                            producer.GetProducerName()));
            }

            return RuleResult.Pass();
        }
    }
}

namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberRegistration;

    public class InsertHasProducerRegistrationNumber : IInsertHasProducerRegistrationNumber
    {
        public RuleResult Evaluate(producerType producer)
        {
            if (producer.status == statusType.I && !string.IsNullOrEmpty(producer.registrationNo))
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "You cannot add a producer registration number (PRN) for {0} when adding the producer for the first time. To add {0} as a new producer, remove the PRN - or if the producer already exists, amend its details using status 'A' not 'I'.",
                            producer.GetProducerName()));
            }

            return RuleResult.Pass();
        }
    }
}

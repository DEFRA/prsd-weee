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
                            "You cannot add a producer registration number (PRN) for {0} when registering the producer for the very first time. To register {0} as a brand new producer, remove the PRN - or if the producer has been registered previously, register them or amend their details using status 'A' not 'I'.",
                            producer.GetProducerName()));
            }

            return RuleResult.Pass();
        }
    }
}

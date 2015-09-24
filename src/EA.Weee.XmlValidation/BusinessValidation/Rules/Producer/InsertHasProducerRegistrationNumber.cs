namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml;
    using Xml.Schemas;

    public class InsertHasProducerRegistrationNumber : IInsertHasProducerRegistrationNumber
    {
        public RuleResult Evaluate(producerType producer)
        {
            if (producer.status == statusType.I && !string.IsNullOrEmpty(producer.registrationNo))
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "A producer registration number for: '{0}' has been entered in the xml file but you are trying to register this producer for the very first time. Check this producer's details. To add this as a new producer remove the producer registration number or to amend  an existing producer details use status \"A\" not \"I\".",
                            producer.GetProducerName()));
            }

            return RuleResult.Pass();
        }
    }
}

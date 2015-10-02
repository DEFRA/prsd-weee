namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml;
    using Xml.Schemas;

    public class AmendmentHasNoProducerRegistrationNumber : IAmendmentHasNoProducerRegistrationNumber
    {
        public RuleResult Evaluate(producerType producer)
        {
            if (producer.status == statusType.A && string.IsNullOrEmpty(producer.registrationNo))
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The producer registration number for '{0}' has been left out of the XML file but the XML file is amending existing producer details. Check this producer's details. To amend this producer add the producer registration number or to add as a brand new producer use status \"I\" not \"A\".",
                            producer.GetProducerName()));
            }

            return RuleResult.Pass();
        }
    }
}

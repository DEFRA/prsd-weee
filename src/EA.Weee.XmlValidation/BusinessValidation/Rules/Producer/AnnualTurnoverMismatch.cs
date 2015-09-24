namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Core.Shared;
    using Xml;
    using Xml.Schemas;

    public class AnnualTurnoverMismatch : IAnnualTurnoverMismatch
    {
        public RuleResult Evaluate(producerType producer)
        {
            var higherBandThanExpected = producer.annualTurnoverBand ==
                                         annualTurnoverBandType.Greaterthanonemillionpounds &&
                                         producer.annualTurnover <= 1000000;

            var lowerBandThanExpected = producer.annualTurnoverBand ==
                                         annualTurnoverBandType.Lessthanorequaltoonemillionpounds &&
                                         producer.annualTurnover > 1000000;

            if (higherBandThanExpected || lowerBandThanExpected)
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The 'annualTurnover' amount you've entered does not match the 'annualTurnoverBand' for {0}, {1}. Please make sure that the 'annualTurnover' amount and 'annualTurnoverBand' for this producer in the XML file are compatible.",
                            producer.GetProducerName(), producer.registrationNo), ErrorLevel.Warning);
            }

            return RuleResult.Pass();
        }
    }
}

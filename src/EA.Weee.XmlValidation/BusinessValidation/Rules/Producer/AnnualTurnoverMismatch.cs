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
                                         producer.annualTurnover.HasValue &&
                                         producer.annualTurnover.Value <= 1000000;

            var lowerBandThanExpected = producer.annualTurnoverBand ==
                                         annualTurnoverBandType.Lessthanorequaltoonemillionpounds &&
                                         producer.annualTurnover.HasValue &&
                                         producer.annualTurnover.Value > 1000000;

            if (higherBandThanExpected || lowerBandThanExpected)
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The annualTurnover amount and annualTurnoverBand for {0} are not compatible. Review your file.",
                            producer.GetProducerName()), ErrorLevel.Warning);
            }

            return RuleResult.Pass();
        }
    }
}

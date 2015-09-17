namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using System;
    using Core.Shared;
    using Core.XmlBusinessValidation;
    using Extensions;
    using Rules;

    public class AnnualTurnoverMismatchEvaluator : IRule<AnnualTurnoverMismatch>
    {
        public RuleResult Evaluate(AnnualTurnoverMismatch ruleData)
        {
            var higherBandThanExpected = ruleData.Producer.annualTurnoverBand ==
                                         annualTurnoverBandType.Greaterthanonemillionpounds &&
                                         ruleData.Producer.annualTurnover <= 1000000;

            var lowerBandThanExpected = ruleData.Producer.annualTurnoverBand ==
                                         annualTurnoverBandType.Lessthanorequaltoonemillionpounds &&
                                         ruleData.Producer.annualTurnover > 1000000;

            if (higherBandThanExpected || lowerBandThanExpected)
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The 'annualTurnover' amount you've entered does not match the 'annualTurnoverBand' for {0}, {1}. Please make sure that the 'annualTurnover' amount and 'annualTurnoverBand' for this producer in the XML file are compatible.",
                            ruleData.Producer.GetProducerName(), ruleData.Producer.registrationNo), ErrorLevel.Warning);
            }

            return RuleResult.Pass();
        }
    }
}

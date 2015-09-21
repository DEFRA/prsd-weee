namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using Core.XmlBusinessValidation;
    using Rules;

    public class ProducerNameRegisteredBeforeEvaluator : IRule<ProducerNameRegisteredBefore>
    {
        public RuleResult Evaluate(ProducerNameRegisteredBefore ruleData)
        {
            // TODO:
            return RuleResult.Pass();
        }
    }
}

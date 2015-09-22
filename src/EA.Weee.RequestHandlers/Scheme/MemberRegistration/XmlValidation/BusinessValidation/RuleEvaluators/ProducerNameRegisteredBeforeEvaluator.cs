namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using Core.XmlBusinessValidation;
    using Extensions;
    using Helpers;
    using QuerySets;
    using Rules;

    public class ProducerNameRegisteredBeforeEvaluator : IRule<ProducerNameRegisteredBefore>
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly ISearchMatch searchMatch;

        public ProducerNameRegisteredBeforeEvaluator(IProducerQuerySet producerQuerySet, ISearchMatch searchMatch)
        {
            this.producerQuerySet = producerQuerySet;
            this.searchMatch = searchMatch;
        }

        public RuleResult Evaluate(ProducerNameRegisteredBefore ruleData)
        {
            // TODO:

            return RuleResult.Pass();
        }
    }
}

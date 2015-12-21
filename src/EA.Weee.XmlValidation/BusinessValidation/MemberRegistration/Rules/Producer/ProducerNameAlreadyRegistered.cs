namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using Helpers;
    using QuerySets;

    public class ProducerNameAlreadyRegistered : IProducerNameAlreadyRegistered
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly ISearchMatcher searchMatch;

        public ProducerNameAlreadyRegistered(IProducerQuerySet producerQuerySet, ISearchMatcher searchMatch)
        {
            this.producerQuerySet = producerQuerySet;
            this.searchMatch = searchMatch;
        }

        public RuleResult Evaluate()
        {
            // TODO:
            return RuleResult.Pass();
        }
    }
}

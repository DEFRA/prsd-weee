namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Helpers;
    using QuerySets;

    public class ProducerNameAlreadyRegistered : IProducerNameAlreadyRegistered
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly ISearchMatch searchMatch;

        public ProducerNameAlreadyRegistered(IProducerQuerySet producerQuerySet, ISearchMatch searchMatch)
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

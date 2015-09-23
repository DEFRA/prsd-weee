namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using FakeItEasy;
    using XmlValidation.BusinessValidation.Helpers;
    using XmlValidation.BusinessValidation.QuerySets;
    using XmlValidation.BusinessValidation.Rules.Producer;

    public class ProducerNameAlreadyRegisteredTests
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly ISearchMatcher searchMatcher;

        public ProducerNameAlreadyRegisteredTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
            searchMatcher = A.Fake<ISearchMatcher>();
        }

        // TODO: Tests

        private ProducerNameAlreadyRegistered Rule()
        {
            return new ProducerNameAlreadyRegistered(producerQuerySet, searchMatcher);
        }
    }
}

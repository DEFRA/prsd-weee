namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using FakeItEasy;
    using XmlValidation.BusinessValidation.MemberRegistration.Helpers;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;

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

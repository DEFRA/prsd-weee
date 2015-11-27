namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Helpers
{
    using XmlValidation.BusinessValidation.MemberRegistration.Helpers;
    using Xunit;

    public class SearchMatcherTests
    {
        [Theory]
        [InlineData("some producer", "some producer")]
        [InlineData("SOME producer", "some producer")]
        [InlineData(null, null)]
        [InlineData("", "")]
        public void MatchByProducerName_ShouldBeCaseInsensitive(string producerName1, string producerName2)
        {
            var result = SearchMatcher().MatchByProducerName(producerName1, producerName2);

            Assert.True(result);
        }

        private SearchMatcher SearchMatcher()
        {
            return new SearchMatcher();
        }
    }
}

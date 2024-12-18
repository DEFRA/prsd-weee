namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Search
{
    using AutoFixture;
    using EA.Weee.Core.Search;
    using EA.Weee.RequestHandlers.Search.FetchSmallProducerSearchResultsForCache;
    using EA.Weee.Requests.Search;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class FetchSmallProducerSearchResultsForCacheHandlerUnitTests : SimpleUnitTestBase
    {
        private readonly FetchSmallProducerSearchResultsForCacheHandler handler;
        private readonly IFetchSmallProducerSearchResultsForCacheDataAccess dataAccess;

        public FetchSmallProducerSearchResultsForCacheHandlerUnitTests()
        {
            dataAccess = A.Fake<IFetchSmallProducerSearchResultsForCacheDataAccess>();

            handler = new FetchSmallProducerSearchResultsForCacheHandler(dataAccess);
        }

        [Fact]
        public async Task HandleAsync_ShouldCallDataAccess_ReturnResults()
        {
            // Arrange
            var smallProducers = TestFixture.CreateMany<SmallProducerSearchResult>().ToList();

            A.CallTo(() => dataAccess.FetchLatestProducers()).Returns(smallProducers);

            // Act
            var result = await handler.HandleAsync(new FetchSmallProducerSearchResultsForCache());

            // Assert
            result.Should().BeEquivalentTo(smallProducers);
        }
    }
}

namespace EA.Weee.Core.Tests.Unit.Admin
{
    using EA.Weee.Core.Admin;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class SimpleProducerSearcherTests
    {
        [Fact]
        public async Task Search_WithMaximumLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            IProducerSearchResultProvider provider = A.Dummy<IProducerSearchResultProvider>();

            SimpleProducerSearcher searcher = new SimpleProducerSearcher(provider);

            // Act
            Func<Task<IList<ProducerSearchResult>>> action = async () => await searcher.Search("test", 0);

            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(action);
        }

        [Fact]
        public async Task Search_WithMaximumOfOne_ReturnsOneResult()
        {
            // Arrange
            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/AA1111AA",
                    Name = "Producer 1"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/BB2222BB",
                    Name = "Producer 2"
                }
            };

            IProducerSearchResultProvider provider = A.Fake<IProducerSearchResultProvider>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            SimpleProducerSearcher searcher = new SimpleProducerSearcher(provider);

            // Act
            IList<ProducerSearchResult> results = await searcher.Search("Producer", 1);

            //Assert
            Assert.NotNull(results);
            Assert.Equal(1, results.Count);
        }

        [Fact]
        public async Task Search_WhereSearchTermPartiallyMatchesPRN_ReturnsResultsWithMatchingPRNs()
        {
            // Arrange
            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/XX0000XX",
                    Name = "Producer 1"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/XX0000AA",
                    Name = "Producer 2"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/AA0000XX",
                    Name = "Producer 3"
                },
            };

            IProducerSearchResultProvider provider = A.Fake<IProducerSearchResultProvider>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            SimpleProducerSearcher searcher = new SimpleProducerSearcher(provider);

            // Act
            IList<ProducerSearchResult> results = await searcher.Search("AA", 10);

            //Assert
            Assert.NotNull(results);
            Assert.DoesNotContain(results, r => r.RegistrationNumber == "WEE/XX0000XX");
            Assert.Contains(results, r => r.RegistrationNumber == "WEE/XX0000AA");
            Assert.Contains(results, r => r.RegistrationNumber == "WEE/AA0000XX");
        }

        [Fact]
        public async Task Search_WhereSearchTermPartiallyMatchesName_ReturnsResultsWithMatchingNames()
        {
            // Arrange
            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/AA1111AA",
                    Name = "xxxxFOOxxxxx"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/BB2222BB",
                    Name = "BARBARBARBAR"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/CC3333CC",
                    Name = "foo foo"
                },
            };

            IProducerSearchResultProvider provider = A.Fake<IProducerSearchResultProvider>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            SimpleProducerSearcher searcher = new SimpleProducerSearcher(provider);

            // Act
            IList<ProducerSearchResult> results = await searcher.Search("foo", 10);

            //Assert
            Assert.NotNull(results);
            Assert.Contains(results, r => r.RegistrationNumber == "WEE/AA1111AA");
            Assert.DoesNotContain(results, r => r.RegistrationNumber == "WEE/BB2222BB");
            Assert.Contains(results, r => r.RegistrationNumber == "WEE/CC3333CC");
        }

        [Fact]
        public async Task Search_WithValidSearchReturnsResultsOrderedByPRN()
        {
            // Arrange
            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/BB2222BB",
                    Name = "A Producer"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/CC3333CC",
                    Name = "Another Producer"
                },
                new ProducerSearchResult()
                {
                    RegistrationNumber = "WEE/AA1111AA",
                    Name = "Some Other Producer"
                },
            };

            IProducerSearchResultProvider provider = A.Fake<IProducerSearchResultProvider>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            SimpleProducerSearcher searcher = new SimpleProducerSearcher(provider);

            // Act
            IList<ProducerSearchResult> results = await searcher.Search("Producer", 10);

            //Assert
            Assert.NotNull(results);
            Assert.Collection(results,
                r1 => Assert.Equal("WEE/AA1111AA", r1.RegistrationNumber),
                r2 => Assert.Equal("WEE/BB2222BB", r2.RegistrationNumber),
                r3 => Assert.Equal("WEE/CC3333CC", r3.RegistrationNumber));
        }
    }
}

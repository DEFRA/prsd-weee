namespace EA.Weee.Core.Tests.Unit.Search.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Search.Simple;
    using FakeItEasy;
    using Xunit;

    public class SimpleSearcherTests
    {
        public class TestableSearchResult : SearchResult
        {
            public string Name { get; set; }
        }

        public class TestableSimpleSearcher : SimpleSearcher<TestableSearchResult>
        {
            public TestableSimpleSearcher(ISearchResultProvider<TestableSearchResult> searchResultProvider)
                : base(searchResultProvider)
            {
            }

            protected override string ResultToString(TestableSearchResult result)
            {
                return result.Name;
            }

            protected override IOrderedEnumerable<TestableSearchResult> OrderResults(IEnumerable<TestableSearchResult> results)
            {
                return results.OrderBy(r => r.Name);
            }
        }

        [Fact]
        public async Task Search_WithMaximumLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            ISearchResultProvider<TestableSearchResult> provider = A.Dummy<ISearchResultProvider<TestableSearchResult>>();

            TestableSimpleSearcher searcher = new TestableSimpleSearcher(provider);

            // Act
            Func<Task<IList<TestableSearchResult>>> action = async () => await searcher.Search("test", 0, false);

            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(action);
        }

        [Fact]
        public async Task Search_WithMaximumOfOne_ReturnsOneResult()
        {
            // Arrange
            List<TestableSearchResult> fakeResults = new List<TestableSearchResult>()
            {
                new TestableSearchResult()
                {
                    Name = "Result 1"
                },
                new TestableSearchResult()
                {
                    Name = "Result 2"
                }
            };

            ISearchResultProvider<TestableSearchResult> provider = A.Fake<ISearchResultProvider<TestableSearchResult>>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            TestableSimpleSearcher searcher = new TestableSimpleSearcher(provider);

            // Act
            IList<TestableSearchResult> results = await searcher.Search("Result", 1, false);

            //Assert
            Assert.NotNull(results);
            Assert.Equal(1, results.Count);
        }

        [Fact]
        public async Task Search_WhereSearchTermPartiallyMatches_ReturnsResultsWithMatchingValues()
        {
            // Arrange
            List<TestableSearchResult> fakeResults = new List<TestableSearchResult>()
            {
                new TestableSearchResult()
                {
                    Name = "XXX 1"
                },
                new TestableSearchResult()
                {
                    Name = "Result 2"
                },
                new TestableSearchResult()
                {
                    Name = "Result 3"
                }
            };

            ISearchResultProvider<TestableSearchResult> provider = A.Fake<ISearchResultProvider<TestableSearchResult>>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            TestableSimpleSearcher searcher = new TestableSimpleSearcher(provider);

            // Act
            IList<TestableSearchResult> results = await searcher.Search("Result", 100, false);

            //Assert
            Assert.NotNull(results);
            Assert.DoesNotContain(results, r => r.Name == "XXX 1");
            Assert.Contains(results, r => r.Name == "Result 2");
            Assert.Contains(results, r => r.Name == "Result 3");
        }

        [Fact]
        public async Task Search_WithValidSearch_OrdersResults()
        {
            // Arrange
            List<TestableSearchResult> fakeResults = new List<TestableSearchResult>()
            {
                new TestableSearchResult()
                {
                    Name = "Result C"
                },
                new TestableSearchResult()
                {
                    Name = "Result A"
                },
                new TestableSearchResult()
                {
                    Name = "Result B"
                }
            };

            ISearchResultProvider<TestableSearchResult> provider = A.Fake<ISearchResultProvider<TestableSearchResult>>();
            A.CallTo(() => provider.FetchAll())
                .Returns(fakeResults);

            TestableSimpleSearcher searcher = new TestableSimpleSearcher(provider);

            // Act
            IList<TestableSearchResult> results = await searcher.Search("Result", 100, false);

            //Assert
            Assert.NotNull(results);
            Assert.Collection(results,
                r1 => Assert.Equal("Result A", r1.Name),
                r2 => Assert.Equal("Result B", r2.Name),
                r3 => Assert.Equal("Result C", r3.Name));
        }
    }
}

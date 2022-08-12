namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.StoredProcedure;
    using FluentAssertions;
    using Mappings;
    using System;
    using System.Linq;
    using Weee.Tests.Core;
    using Xunit;

    public class ObligationSummaryTotalsMapTests : SimpleUnitTestBase
    {
        private readonly ObligationSummaryTotalsMap mapper;

        public ObligationSummaryTotalsMapTests()
        {
            mapper = new ObligationSummaryTotalsMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => mapper.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_DataShouldBeMapped()
        {
            //arrange
            var source = TestFixture.CreateMany<ObligationEvidenceSummaryTotalsData>().ToList();

            //act
            var result = mapper.Map(source);

            //assert
            source.ForEach((s) =>
            {
                result.ObligationEvidenceValues.Should().Contain(o => o.Obligation == s.Obligation &&
                                                                      o.CategoryId.ToInt() == s.CategoryId.ToInt() &&
                                                                      o.TransferredOut == s.TransferredOut &&
                                                                      o.Difference == s.ObligationDifference &&
                                                                      o.Evidence == s.Evidence &&
                                                                      o.TransferredIn == s.TransferredIn &&
                                                                      o.Reuse == s.Reuse);
            });
        }
    }
}

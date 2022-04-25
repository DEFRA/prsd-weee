namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using DataAccess.StoredProcedure;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class AatfSummaryTotalsMapTests
    {
        private readonly AatfSummaryTotalsMap map;
        private readonly Fixture fixture;

        public AatfSummaryTotalsMapTests()
        {
            fixture = new Fixture();

            map = new AatfSummaryTotalsMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_ItemsShouldBeMapped()
        {
            //arrange
            var source = fixture.CreateMany<AatfEvidenceSummaryTotalsData>().ToList();

            //act
            var result = map.Map(source);

            //assert
            result.Count.Should().Be(source.Count);
            result.Should().BeEquivalentTo(source.Select(e =>
                new EvidenceSummaryTonnageData((WeeeCategory)e.CategoryId, e.Received, e.Reused)));
        }
    }
}

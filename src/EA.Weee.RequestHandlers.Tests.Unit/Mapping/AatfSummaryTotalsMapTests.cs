namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using DataAccess.StoredProcedure;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Helpers;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfSummaryTotalsMapTests : SimpleUnitTestBase
    {
        private readonly AatfSummaryTotalsMap map;

        public AatfSummaryTotalsMapTests()
        {
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
            var source = TestFixture.CreateMany<AatfEvidenceSummaryTotalsData>().ToList();

            //act
            var result = map.Map(source);

            //assert
            result.Count.Should().Be(source.Count);
            result.Should().BeEquivalentTo(source.Select(e =>
                new EvidenceSummaryTonnageData((WeeeCategory)e.CategoryId, e.ApprovedReceived, e.ApprovedReused)));
        }

        [Fact]
        public void Map_GivenSourceThatContainsCategoryIdNotMatchingCategoryEnum_ItemsShouldBeMapped()
        {
            //arrange

            // only want to map valid WeeeCategory values, so ensure higher values are not mapped
            var categories = Enum.GetValues(typeof(WeeeCategory)).Cast<int>();
            var maxId = categories.Max();

            var source = new List<AatfEvidenceSummaryTotalsData>()
            {
                new AatfEvidenceSummaryTotalsData()
                {
                    CategoryId = (Domain.Lookup.WeeeCategory)(maxId + 1)
                }
            };

            foreach (var value in EnumHelper.GetValues(typeof(WeeeCategory)))
            {
                source.Add(new AatfEvidenceSummaryTotalsData()
                {
                    CategoryId = (Domain.Lookup.WeeeCategory)value.Key
                });
            }

            //act
            var result = map.Map(source);

            //assert
            result.Should().HaveCount(categories.Count());
        }
    }
}

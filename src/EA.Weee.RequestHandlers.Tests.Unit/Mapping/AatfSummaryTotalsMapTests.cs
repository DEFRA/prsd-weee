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

            // ensure only valid WeeeCategory values are mapped by adding an invalid category (id too high)
            // as well as all valid categories
            var categories = EnumHelper.GetValues(typeof(WeeeCategory));
            var maxCategoryId = categories.Max(x => x.Key);

            var source = new List<AatfEvidenceSummaryTotalsData>()
            {
                new AatfEvidenceSummaryTotalsData()
                {
                    CategoryId = (Domain.Lookup.WeeeCategory)(maxCategoryId + 1)
                }
            };

            foreach (var value in categories)
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

namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class NonObligatedDataToNonObligatedValueMapTests
    {
        private readonly NonObligatedDataToNonObligatedValueMap map;

        public NonObligatedDataToNonObligatedValueMapTests()
        {
            map = new NonObligatedDataToNonObligatedValueMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullNonObligatedCategoryValues_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedDataValues = A.Fake<IList<NonObligatedData>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullNonObligatedDataValues_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedCategoryValues = A.Fake<IList<NonObligatedCategoryValue>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainCorrectNumberOfValues()
        {
            var nonObligatedValues = new List<NonObligatedCategoryValue>() { new NonObligatedCategoryValue() };

            var result = map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedCategoryValues = nonObligatedValues, NonObligatedDataValues = A.Fake<IList<NonObligatedData>>() });

            result.Count.Should().Be(1);
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainValidMappedValues()
        {
            var nonObligatedValues = new List<NonObligatedCategoryValue>()
            {
                new NonObligatedCategoryValue() { CategoryDisplay = "obligated1display", CategoryId = 1 },
                new NonObligatedCategoryValue() { CategoryDisplay = "obligated2display", CategoryId = 2 }
            };

            var nonObligatedDataValues = new List<NonObligatedData>()
            {
                new NonObligatedData(1, 0, false, Guid.NewGuid()),
                new NonObligatedData(2, 0, false, Guid.NewGuid())
            };

            var result = map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedCategoryValues = nonObligatedValues, NonObligatedDataValues = nonObligatedDataValues });

            result.ElementAt(0).CategoryDisplay.Should().Be(nonObligatedValues.ElementAt(0).CategoryDisplay);
            result.ElementAt(0).Id.Should().Be(nonObligatedDataValues.ElementAt(0).Id);
            result.ElementAt(1).CategoryDisplay.Should().Be(nonObligatedValues.ElementAt(1).CategoryDisplay);
            result.ElementAt(1).Id.Should().Be(nonObligatedDataValues.ElementAt(1).Id);
        }

        [Fact]
        public void Map_GivenMappingObjectsWithNoMatchingIds_MappedObjectShouldNotContainMappedValues()
        {
            var nonObligatedValues = new List<NonObligatedCategoryValue>()
            {
                new NonObligatedCategoryValue() { CategoryDisplay = "obligated1display", CategoryId = 1 },
                new NonObligatedCategoryValue() { CategoryDisplay = "obligated2display", CategoryId = 2 }
            };

            var weeeDataValues = new List<NonObligatedData>()
            {
                new NonObligatedData(3, 0, false, Guid.NewGuid()),
                new NonObligatedData(4, 0, false, Guid.NewGuid())
            };

            var result = map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedCategoryValues = nonObligatedValues, NonObligatedDataValues = weeeDataValues });

            result.ElementAt(0).CategoryDisplay.Should().Be(nonObligatedValues.ElementAt(0).CategoryDisplay);
            result.ElementAt(0).Id.Should().Be(nonObligatedValues.ElementAt(0).Id);
            result.ElementAt(1).CategoryDisplay.Should().Be(nonObligatedValues.ElementAt(1).CategoryDisplay);
            result.ElementAt(1).Id.Should().Be(nonObligatedValues.ElementAt(1).Id);
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainValidTonnageValues()
        {
            var obligatedValues = new List<NonObligatedCategoryValue>()
            {
                new NonObligatedCategoryValue() { CategoryId = 1 },
                new NonObligatedCategoryValue() { CategoryId = 2 },
                new NonObligatedCategoryValue() { CategoryId = 3 },
                new NonObligatedCategoryValue() { CategoryId = 4 },
                new NonObligatedCategoryValue() { CategoryId = 5 },
                new NonObligatedCategoryValue() { CategoryId = 6 },
                new NonObligatedCategoryValue() { CategoryId = 7 },
                new NonObligatedCategoryValue() { CategoryId = 8 },
                new NonObligatedCategoryValue() { CategoryId = 9 }
            };

            var weeeDataValues = new List<NonObligatedData>()
            {
                new NonObligatedData(1, null, false, Guid.NewGuid()),
                new NonObligatedData(2, 1m, false, Guid.NewGuid()),
                new NonObligatedData(3, 1000m, false, Guid.NewGuid()),
                new NonObligatedData(4, 2m, false, Guid.NewGuid()),
                new NonObligatedData(5, 6.5m, false, Guid.NewGuid()),
                new NonObligatedData(6, 897.555m, false, Guid.NewGuid()),
                new NonObligatedData(7, 98.02m, false, Guid.NewGuid()),
                new NonObligatedData(8, 33.3m, false, Guid.NewGuid()),
                new NonObligatedData(9, 909.007m, false, Guid.NewGuid()),
            };

            var result = map.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedCategoryValues = obligatedValues, NonObligatedDataValues = weeeDataValues });

            result.ElementAt(0).Tonnage.Should().Be(string.Empty);
            result.ElementAt(0).Tonnage.Should().Be(string.Empty);
            result.ElementAt(1).Tonnage.Should().Be("1");
            result.ElementAt(1).Tonnage.Should().Be("1");
            result.ElementAt(2).Tonnage.Should().Be("1000");
            result.ElementAt(2).Tonnage.Should().Be("1000");
            result.ElementAt(3).Tonnage.Should().Be("2");
            result.ElementAt(3).Tonnage.Should().Be("2");
            result.ElementAt(4).Tonnage.Should().Be("6.5");
            result.ElementAt(4).Tonnage.Should().Be("6.5");
            result.ElementAt(5).Tonnage.Should().Be("897.555");
            result.ElementAt(5).Tonnage.Should().Be("897.555");
            result.ElementAt(6).Tonnage.Should().Be("98.02");
            result.ElementAt(6).Tonnage.Should().Be("98.02");
            result.ElementAt(7).Tonnage.Should().Be("33.3");
            result.ElementAt(7).Tonnage.Should().Be("33.3");
            result.ElementAt(8).Tonnage.Should().Be("909.007");
            result.ElementAt(8).Tonnage.Should().Be("909.007");
        }
    }
}

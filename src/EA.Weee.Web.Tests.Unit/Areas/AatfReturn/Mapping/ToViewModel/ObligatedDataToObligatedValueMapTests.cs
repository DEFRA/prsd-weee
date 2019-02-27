namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ObligatedDataToObligatedValueMapTests
    {
        private readonly ObligatedDataToObligatedValueMap map;

        public ObligatedDataToObligatedValueMapTests()
        {
            map = new ObligatedDataToObligatedValueMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullWeeeDataValues_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new ObligatedDataToObligatedValueMapTransfer() {ObligatedCategoryValues = A.Fake<IList<ObligatedCategoryValue>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullObligatedValues_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new ObligatedDataToObligatedValueMapTransfer() { WeeeDataValues = A.Fake<IList<WeeeObligatedData>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainCorrectNumberOfValues()
        {
            var obligatedValues = new List<ObligatedCategoryValue>() { new ObligatedCategoryValue() };

            var result = map.Map(new ObligatedDataToObligatedValueMapTransfer() { ObligatedCategoryValues = obligatedValues, WeeeDataValues = A.Fake<IList<WeeeObligatedData>>() });

            result.Count.Should().Be(1);
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainValidMappedValues()
        {
            var obligatedValues = new List<ObligatedCategoryValue>()
            {
                new ObligatedCategoryValue() { CategoryDisplay = "obligated1display", CategoryId = 1 },
                new ObligatedCategoryValue() { CategoryDisplay = "obligated2display", CategoryId = 2 }
            };

            var weeeDataValues = new List<WeeeObligatedData>()
            {
                new WeeeObligatedData() { Id = Guid.NewGuid(), CategoryId = 1 },
                new WeeeObligatedData() { Id = Guid.NewGuid(), CategoryId = 2 }
            };

            var result = map.Map(new ObligatedDataToObligatedValueMapTransfer() { ObligatedCategoryValues = obligatedValues, WeeeDataValues = weeeDataValues });

            result.ElementAt(0).CategoryDisplay.Should().Be(obligatedValues.ElementAt(0).CategoryDisplay);
            result.ElementAt(0).Id.Should().Be(weeeDataValues.ElementAt(0).Id);
            result.ElementAt(1).CategoryDisplay.Should().Be(obligatedValues.ElementAt(1).CategoryDisplay);
            result.ElementAt(1).Id.Should().Be(weeeDataValues.ElementAt(1).Id);
        }

        [Fact]
        public void Map_GivenMappingObjectsWithNoMatchingIds_MappedObjectShouldNotContainMappedValues()
        {
            var obligatedValues = new List<ObligatedCategoryValue>()
            {
                new ObligatedCategoryValue() { CategoryDisplay = "obligated1display", CategoryId = 1 },
                new ObligatedCategoryValue() { CategoryDisplay = "obligated2display", CategoryId = 2 }
            };

            var weeeDataValues = new List<WeeeObligatedData>()
            {
                new WeeeObligatedData() { Id = Guid.NewGuid(), CategoryId = 3 },
                new WeeeObligatedData() { Id = Guid.NewGuid(), CategoryId = 4 }
            };

            var result = map.Map(new ObligatedDataToObligatedValueMapTransfer() { ObligatedCategoryValues = obligatedValues, WeeeDataValues = weeeDataValues });

            result.ElementAt(0).CategoryDisplay.Should().Be(obligatedValues.ElementAt(0).CategoryDisplay);
            result.ElementAt(0).Id.Should().Be(obligatedValues.ElementAt(0).Id);
            result.ElementAt(1).CategoryDisplay.Should().Be(obligatedValues.ElementAt(1).CategoryDisplay);
            result.ElementAt(1).Id.Should().Be(obligatedValues.ElementAt(1).Id);
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainValidTonnageValues()
        {
            var obligatedValues = new List<ObligatedCategoryValue>()
            {
                new ObligatedCategoryValue() { CategoryId = 1 },
                new ObligatedCategoryValue() { CategoryId = 2 },
                new ObligatedCategoryValue() { CategoryId = 3 },
                new ObligatedCategoryValue() { CategoryId = 4 },
                new ObligatedCategoryValue() { CategoryId = 5 },
                new ObligatedCategoryValue() { CategoryId = 6 },
                new ObligatedCategoryValue() { CategoryId = 7 },
                new ObligatedCategoryValue() { CategoryId = 8 },
                new ObligatedCategoryValue() { CategoryId = 9 }
            };

            var weeeDataValues = new List<WeeeObligatedData>()
            {
                new WeeeObligatedData() { CategoryId = 1, B2B = null, B2C = null},
                new WeeeObligatedData() { CategoryId = 2, B2B = 1m, B2C = 1m },
                new WeeeObligatedData() { CategoryId = 3, B2B = 1000m, B2C = 1000m },
                new WeeeObligatedData() { CategoryId = 4, B2B = 0m, B2C = 0m },
                new WeeeObligatedData() { CategoryId = 5, B2B = 1.0000m, B2C = 1.0000m },
                new WeeeObligatedData() { CategoryId = 6, B2B = 1.001m, B2C = 1.001m },
                new WeeeObligatedData() { CategoryId = 7, B2B = 1.1m, B2C = 1.1m },
                new WeeeObligatedData() { CategoryId = 8, B2B = 10m, B2C = 10m },
                new WeeeObligatedData() { CategoryId = 9, B2B = 10.01m, B2C = 10.01m },
            };

            var result = map.Map(new ObligatedDataToObligatedValueMapTransfer() { ObligatedCategoryValues = obligatedValues, WeeeDataValues = weeeDataValues });

            result.ElementAt(0).B2B.Should().Be(string.Empty);
            result.ElementAt(0).B2C.Should().Be(string.Empty);
            result.ElementAt(1).B2B.Should().Be("1.000");
            result.ElementAt(1).B2C.Should().Be("1.000");
            result.ElementAt(2).B2B.Should().Be("1,000.000");
            result.ElementAt(2).B2C.Should().Be("1,000.000");
            result.ElementAt(3).B2B.Should().Be("0.000");
            result.ElementAt(3).B2C.Should().Be("0.000");
            result.ElementAt(4).B2B.Should().Be("1.000");
            result.ElementAt(4).B2C.Should().Be("1.000");
            result.ElementAt(5).B2B.Should().Be("1.001");
            result.ElementAt(5).B2C.Should().Be("1.001");
            result.ElementAt(6).B2B.Should().Be("1.100");
            result.ElementAt(6).B2C.Should().Be("1.100");
            result.ElementAt(7).B2B.Should().Be("10.000");
            result.ElementAt(7).B2C.Should().Be("10.000");
            result.ElementAt(8).B2B.Should().Be("10.010");
            result.ElementAt(8).B2C.Should().Be("10.010");
        }
    }       
}

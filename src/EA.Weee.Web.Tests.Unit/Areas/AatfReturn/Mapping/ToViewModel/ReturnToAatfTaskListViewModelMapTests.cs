namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToAatfTaskListViewModelMapTests
    {
        private readonly ReturnToReturnViewModelMap map;
        private readonly string zeroTonnage = "0.000";
        private Guid mapperTestId;
        private int mapperTestYear;
        private Quarter mapperTestQuarter;
        private QuarterWindow mapperTestQuarterWindow;
        private string mapperTestPeriod;
        private List<NonObligatedData> mapperTestNonObligatedData;

        public ReturnToAatfTaskListViewModelMapTests()
        {
            map = new ReturnToReturnViewModelMap();
            mapperTestId = new Guid();
            mapperTestYear = 2019;
            mapperTestQuarter = new Quarter(mapperTestYear, QuarterType.Q1);
            mapperTestQuarterWindow = new QuarterWindow(new DateTime(mapperTestYear, 1, 1), new DateTime(mapperTestYear, 3, 31));
            mapperTestPeriod = "Q1 Jan - Mar";
            mapperTestNonObligatedData = new List<NonObligatedData>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            mapperTestNonObligatedData.Add(new NonObligatedData(0, (decimal)1.234, false));
            mapperTestNonObligatedData.Add(new NonObligatedData(0, (decimal)1.234, true));

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData
            };

            var result = map.Map(returnData);

            result.Quarter.Should().Be(mapperTestQuarter.Q.ToString());
            result.Year.Should().Be(mapperTestYear.ToString());
            result.Period.Should().Be(mapperTestPeriod);
            result.NonObligatedTonnageTotal.Should().Be("1.234");
            result.NonObligatedTonnageTotalDcf.Should().Be("1.234");
        }

        [Fact]
        public void Map_GivenNullObligatedData_ReturnsZeroTonnage()
        {
            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData
            };

            var result = map.Map(returnData);

            result.Quarter.Should().Be(mapperTestQuarter.Q.ToString());
            result.Year.Should().Be(mapperTestYear.ToString());
            result.Period.Should().Be(mapperTestPeriod);
            result.NonObligatedTonnageTotal.Should().Be(zeroTonnage);
            result.NonObligatedTonnageTotalDcf.Should().Be(zeroTonnage);
        }

        [Fact]
        public void Map_GivenNullTonnage_ReturnsZeroTonnage()
        {
            mapperTestNonObligatedData.Add(new NonObligatedData(0, null, false));
            mapperTestNonObligatedData.Add(new NonObligatedData(0, null, true));

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData
            };

            var result = map.Map(returnData);

            result.Quarter.Should().Be(mapperTestQuarter.Q.ToString());
            result.Year.Should().Be(mapperTestYear.ToString());
            result.Period.Should().Be(mapperTestPeriod);
            result.NonObligatedTonnageTotal.Should().Be(zeroTonnage);
            result.NonObligatedTonnageTotalDcf.Should().Be(zeroTonnage);
        }
    }
}

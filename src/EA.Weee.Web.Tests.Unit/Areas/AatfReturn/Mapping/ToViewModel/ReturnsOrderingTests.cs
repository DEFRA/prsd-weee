namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnsOrderingTests
    {
        private readonly ReturnsOrdering ordering;

        public ReturnsOrderingTests()
        {
            ordering = new ReturnsOrdering();
        }

        [Fact]
        public void Map_GivenReturnsData_ViewModelReturnsShouldInitiallyBeOrderedByYear()
        {
            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2018, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                }
            };

            var result = ordering.Order(returnData);

            result.ElementAt(0).Quarter.Year.Should().Be(2020);
            result.ElementAt(1).Quarter.Year.Should().Be(2019);
            result.ElementAt(2).Quarter.Year.Should().Be(2018);
        }

        [Fact]
        public void Map_GivenReturnsData_ViewModelReturnsShouldBeOrderedByYearThenQuarter()
        {
            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q2), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q4), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q3), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                }
            };

            var result = ordering.Order(returnData);

            result.ElementAt(0).Quarter.Q.Should().Be(QuarterType.Q4);
            result.ElementAt(1).Quarter.Q.Should().Be(QuarterType.Q3);
            result.ElementAt(2).Quarter.Q.Should().Be(QuarterType.Q2);
            result.ElementAt(3).Quarter.Q.Should().Be(QuarterType.Q1);
        }

        [Fact]
        public void Map_GivenReturnsData_ViewModelReturnsShouldBeOrderedByYearThenQuarterThenCreatedDate()
        {
            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    CreatedDate = new DateTime(2017),
                    Quarter = new Quarter(2020, QuarterType.Q4), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    CreatedDate = new DateTime(2019),
                    Quarter = new Quarter(2020, QuarterType.Q4), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    CreatedDate = new DateTime(2018),
                    Quarter = new Quarter(2020, QuarterType.Q4), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                },
                new ReturnData()
                {
                    CreatedDate = new DateTime(2020),
                    Quarter = new Quarter(2020, QuarterType.Q4), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1), (int)Core.DataReturns.QuarterType.Q1)
                }
            };

            var result = ordering.Order(returnData);

            result.ElementAt(0).CreatedDate.Should().Be(new DateTime(2020));
            result.ElementAt(1).CreatedDate.Should().Be(new DateTime(2019));
            result.ElementAt(2).CreatedDate.Should().Be(new DateTime(2018));
            result.ElementAt(3).CreatedDate.Should().Be(new DateTime(2017));
        }
    }
}

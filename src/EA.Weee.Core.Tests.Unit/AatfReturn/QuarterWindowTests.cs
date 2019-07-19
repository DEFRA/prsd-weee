namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using DataReturns;
    using FluentAssertions;
    using Xunit;

    public class QuarterWindowTests
    {
        private const string Q1 = "2018-04-01";
        private const string Q2 = "2018-07-01";
        private const string Q3 = "2018-10-01";
        private const string Q4 = "2019-01-01";
        private readonly DateTime windowClosedDate = new DateTime(2019, 03, 16);
     
        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void Constructor_GivenConstructorParameters_PropertiesShouldBeSet(DateTime windowOpenDate, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            quarter.QuarterType.Should().Be(quarterType);
            quarter.WindowOpenDate.Should().Be(windowOpenDate);
            quarter.WindowClosedDate.Should().Be(windowClosedDate);
        }

        [Fact]
        public void Constructor_GivenQuarterOneConstructorParameters_QuarterStartEndPropertiesShouldBeSet()
        {
            var quarter = new QuarterWindow(Convert.ToDateTime(Q1), windowClosedDate, QuarterType.Q1);

            quarter.QuarterStart.Should().Be(new DateTime(2018, 1, 1));
            quarter.QuarterEnd.Should().Be(new DateTime(2018, 3, 31));
        }

        [Fact]
        public void Constructor_GivenQuarterTwoConstructorParameters_QuarterStartEndPropertiesShouldBeSet()
        {
            var quarter = new QuarterWindow(Convert.ToDateTime(Q2), windowClosedDate, QuarterType.Q2);

            quarter.QuarterStart.Should().Be(new DateTime(2018, 4, 1));
            quarter.QuarterEnd.Should().Be(new DateTime(2018, 6, 30));
        }

        [Fact]
        public void Constructor_GivenQuarterThreeConstructorParameters_QuarterStartEndPropertiesShouldBeSet()
        {
            var quarter = new QuarterWindow(Convert.ToDateTime(Q3), windowClosedDate, QuarterType.Q3);

            quarter.QuarterStart.Should().Be(new DateTime(2018, 7, 1));
            quarter.QuarterEnd.Should().Be(new DateTime(2018, 9, 30));
        }

        [Fact]
        public void Constructor_GivenQuarterFourConstructorParameters_QuarterStartEndPropertiesShouldBeSet()
        {
            var quarter = new QuarterWindow(Convert.ToDateTime(Q4), windowClosedDate, QuarterType.Q4);

            quarter.QuarterStart.Should().Be(new DateTime(2018, 10, 1));
            quarter.QuarterEnd.Should().Be(new DateTime(2018, 12, 31));
        }

        [Theory]
        [InlineData(Q1, 31, 03, QuarterType.Q1)]
        [InlineData(Q2, 30, 06, QuarterType.Q2)]
        [InlineData(Q3, 30, 09, QuarterType.Q3)]
        [InlineData(Q4, 31, 12, QuarterType.Q4)]
        public void IsOpen_DayBeforeReportingWindowOpens_ReturnsFalse(DateTime windowOpenDate, int currentDay, int currentMonth, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);
            var date = new DateTime(2018, currentMonth, currentDay);

            var result = quarter.IsOpen(date);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(Q1, 04, 2018, QuarterType.Q1)]
        [InlineData(Q2, 07, 2018, QuarterType.Q2)]
        [InlineData(Q3, 10, 2018, QuarterType.Q3)]
        [InlineData(Q4, 01, 2019, QuarterType.Q4)]
        public void IsOpen_DayReportingWindowOpens_ReturnsTrue(DateTime windowOpenDate, int currentMonth, int currentYear, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            var date = new DateTime(currentYear, currentMonth, 01);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, 04, 2018, QuarterType.Q1)]
        [InlineData(Q2, 07, 2018, QuarterType.Q2)]
        [InlineData(Q3, 10, 2018, QuarterType.Q3)]
        [InlineData(Q4, 01, 2019, QuarterType.Q4)]
        public void IsOpen_DayAfterReportingWindowOpens_ReturnsTrue(DateTime windowOpenDate, int currentMonth, int currentYear, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            var date = new DateTime(currentYear, currentMonth, 02);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpen_DayBeforeReportingWindowCloses_ReturnsTrue(DateTime windowOpenDate, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            var date = new DateTime(2019, 03, 15);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpen_DayReportingWindowCloses_ReturnsTrue(DateTime windowOpenDate, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            var date = new DateTime(2019, 03, 16);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpen_DayAfterReportingWindowCloses_ReturnsFalse(DateTime windowOpenDate, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(windowOpenDate, windowClosedDate, quarterType);

            var date = new DateTime(2019, 03, 17);

            var result = quarter.IsOpen(date);

            result.Should().BeFalse();
        }
    }
}

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

        public QuarterWindowTests()
        {
        }

        [Theory]
        [InlineData(Q1, 31, 03, QuarterType.Q1)]
        [InlineData(Q2, 30, 06, QuarterType.Q2)]
        [InlineData(Q3, 30, 09, QuarterType.Q3)]
        [InlineData(Q4, 31, 12, QuarterType.Q4)]
        public void IsOpenForReporting_DayBeforeReportingWindowOpens_ReturnsFalse(DateTime quarterStart, int currentDay, int currentMonth, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);
            var date = new DateTime(2018, currentMonth, currentDay);

            var result = quarter.IsOpen(date);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(Q1, 04, 2018, QuarterType.Q1)]
        [InlineData(Q2, 07, 2018, QuarterType.Q2)]
        [InlineData(Q3, 10, 2018, QuarterType.Q3)]
        [InlineData(Q4, 01, 2019, QuarterType.Q4)]
        public void IsOpenForReporting_DayReportingWindowOpens_ReturnsTrue(DateTime quarterStart, int currentMonth, int currentYear, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);

            var date = new DateTime(currentYear, currentMonth, 01);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, 04, 2018, QuarterType.Q1)]
        [InlineData(Q2, 07, 2018, QuarterType.Q2)]
        [InlineData(Q3, 10, 2018, QuarterType.Q3)]
        [InlineData(Q4, 01, 2019, QuarterType.Q4)]
        public void IsOpenForReporting_DayAfterReportingWindowOpens_ReturnsTrue(DateTime quarterStart, int currentMonth, int currentYear, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);

            var date = new DateTime(currentYear, currentMonth, 02);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpenForReporting_DayBeforeReportingWindowCloses_ReturnsTrue(DateTime quarterStart, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);

            var date = new DateTime(2019, 03, 15);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpenForReporting_DayReportingWindowCloses_ReturnsTrue(DateTime quarterStart, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);

            var date = new DateTime(2019, 03, 16);

            var result = quarter.IsOpen(date);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(Q1, QuarterType.Q1)]
        [InlineData(Q2, QuarterType.Q2)]
        [InlineData(Q3, QuarterType.Q3)]
        [InlineData(Q4, QuarterType.Q4)]
        public void IsOpenForReporting_DayAfterReportingWindowCloses_ReturnsFalse(DateTime quarterStart, QuarterType quarterType)
        {
            var quarter = new QuarterWindow(quarterStart, new DateTime(2019, 03, 16), quarterType);

            var date = new DateTime(2019, 03, 17);

            var result = quarter.IsOpen(date);

            result.Should().BeFalse();
        }
    }
}

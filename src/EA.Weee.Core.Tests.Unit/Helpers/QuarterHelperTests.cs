namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using Xunit;

    public class QuaterHelperTests
    {
        private const string Q1 = "2018-01-01";
        private const string Q2 = "2018-04-01";
        private const string Q3 = "2018-07-01";
        private const string Q4 = "2018-10-01";
        private readonly Fixture fixture;

        public QuaterHelperTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData(Q1, 31, 03)]
        [InlineData(Q2, 30, 06)]
        [InlineData(Q3, 30, 09)]
        [InlineData(Q4, 31, 12)]
        public void IsOpenForReporting_DayBeforeReportingWindowOpens_ReturnsFalse(DateTime quarterStart, int currentDay, int currentMonth)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(2018, currentMonth, currentDay));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.False(result);
        }

        [Theory]
        [InlineData(Q1, 04, 2018)]
        [InlineData(Q2, 07, 2018)]
        [InlineData(Q3, 10, 2018)]
        [InlineData(Q4, 01, 2019)]
        public void IsOpenForReporting_DayReportingWindowOpens_ReturnsTrue(DateTime quarterStart, int currentMonth, int currentYear)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(currentYear, currentMonth, 01));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.True(result);
        }

        [Theory]
        [InlineData(Q1, 04, 2018)]
        [InlineData(Q2, 07, 2018)]
        [InlineData(Q3, 10, 2018)]
        [InlineData(Q4, 01, 2019)]
        public void IsOpenForReporting_DayAfterReportingWindowOpens_ReturnsTrue(DateTime quarterStart, int currentMonth, int currentYear)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(currentYear, currentMonth, 02));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.True(result);
        }

        [Theory]
        [InlineData(Q1)]
        [InlineData(Q2)]
        [InlineData(Q3)]
        [InlineData(Q4)]
        public void IsOpenForReporting_DayBeforeReportingWindowCloses_ReturnsTrue(DateTime quarterStart)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(2019, 03, 15));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.True(result);
        }

        [Theory]
        [InlineData(Q1)]
        [InlineData(Q2)]
        [InlineData(Q3)]
        [InlineData(Q4)]
        public void IsOpenForReporting_DayReportingWindowCloses_ReturnsTrue(DateTime quarterStart)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(2019, 03, 16));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.True(result);
        }

        [Theory]
        [InlineData(Q1)]
        [InlineData(Q2)]
        [InlineData(Q3)]
        [InlineData(Q4)]
        public void IsOpenForReporting_DayAfterReportingWindowCloses_ReturnsFalse(DateTime quarterStart)
        {
            var quarter = new QuarterWindow(quarterStart, fixture.Create<DateTime>());

            SystemTime.Freeze(new DateTime(2019, 03, 17));
            var result = QuarterHelper.IsOpenForReporting(quarter);
            SystemTime.Unfreeze();

            Assert.False(result);
        }
    }
}

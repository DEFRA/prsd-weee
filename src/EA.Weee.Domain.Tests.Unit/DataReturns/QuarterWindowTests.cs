namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Xunit;

    public class QuarterWindowTests
    {
        [Fact]
        public void CurrentDateTimeIsOneSecondBeforeWindowOpens_IsBeforeWindow()
        {
            var startDate = new DateTime(2016, 04, 01, 0, 0, 0);
            var currentDate = new DateTime(2016, 03, 31, 23, 59, 59);

            var quarterWindow = new QuarterWindow(startDate, A<DateTime>._);

            var result = quarterWindow.IsBeforeWindow(currentDate);

            Assert.True(result);
        }

        [Fact]
        public void CurrentDateTimeIsExactlyWindowStartDate_IsNotBeforeWindow()
        {
            var startDate = new DateTime(2016, 04, 01, 0, 0, 0);
            var currentDate = new DateTime(2016, 04, 01, 0, 0, 0);

            var quarterWindow = new QuarterWindow(startDate, A<DateTime>._);

            var result = quarterWindow.IsBeforeWindow(currentDate);

            Assert.False(result);
        }

        [Fact]
        public void CurrentDateTimeIsOneSecondBeforeDayAfterWindowEndDate_IsNotAfterWindow()
        {
            var endDate = new DateTime(2016, 07, 01, 0, 0, 0);
            var currentDate = new DateTime(2016, 07, 01, 23, 59, 59);

            var quarterWindow = new QuarterWindow(A<DateTime>._, endDate);

            var result = quarterWindow.IsAfterWindow(currentDate);

            Assert.False(result);
        }

        [Fact]
        public void CurrentDateTimeExactlyOneDayAfterWindowEndDate_IsAfterWindow()
        {
            var endDate = new DateTime(2016, 07, 01, 0, 0, 0);
            var currentDate = new DateTime(2016, 07, 02, 0, 0, 0);

            var quarterWindow = new QuarterWindow(A<DateTime>._, endDate);

            var result = quarterWindow.IsAfterWindow(currentDate);

            Assert.True(result);
        }
    }
}

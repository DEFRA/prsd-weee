namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation.XmlBusinessRules
{
    using System;
    using System.Linq;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.DataReturns;
    using Factories;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.DataReturns.BusinessValidation.Rules;
    using Xml.DataReturns;
    using Xunit;

    public class SubmissionWindowClosedTests
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public SubmissionWindowClosedTests()
        {
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
        }

        [Fact]
        public async void UsingFixedDate_AndDateIsWithinWindow_NoValidationError()
        {
            var systemData = new TestSystemData();
            systemData.UpdateFixedCurrentDate(new DateTime(2016, 01, 01, 0, 0, 0));
            systemData.ToggleFixedCurrentDateUsage(true);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var windowStart = new DateTime(2016, 1, 1, 0, 0, 0);
            var windowEnd = new DateTime(2016, 1, 2, 0, 0, 0);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._))
                .Returns(new QuarterWindow(windowStart, windowEnd));

            var result = await
                SubmissionWindowClosed()
                    .Validate(new Quarter(2016, QuarterType.Q1));

            Assert.Empty(result);
        }

        [Fact]
        public async void NotUsingFixedDate_TimeNowIsWithinQuarterWindow_NoValidationError()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 1, 1, 0, 0, 0);

            var windowStart = new DateTime(2016, 1, 1, 0, 0, 0);
            var windowEnd = new DateTime(2016, 1, 2, 0, 0, 0);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._))
                .Returns(new QuarterWindow(windowStart, windowEnd));

            SystemTime.Freeze(timeNow, true);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new Quarter(2016, QuarterType.Q1));

            SystemTime.Unfreeze();

            Assert.Empty(result);
        }

        [Fact]
        public async void NotUsingFixedDate_TimeNowBeforeQuarterWindow_ValidationError_StatingWindowNotYetOpened()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2015, 12, 31, 23, 59, 59);

            var windowStart = new DateTime(2016, 1, 1, 0, 0, 0);
            var windowEnd = new DateTime(2016, 1, 2, 0, 0, 0);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._))
                .Returns(new QuarterWindow(windowStart, windowEnd));

            SystemTime.Freeze(timeNow, true);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new Quarter(2016, QuarterType.Q1));

            SystemTime.Unfreeze();

            Assert.Single(result);

            var error = result.Single();

            Assert.Equal(Core.Shared.ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("not yet opened", error.Description);
            Assert.Contains("2016", error.Description);
        }

        [Fact]
        public async void NotUsingFixedComplianceYearAndQuarter_TimeAfterQuarterWindow_ValidationError_StatingWindowHasClosed()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 1, 1, 0, 0, 0);

            var windowStart = new DateTime(2015, 1, 1, 0, 0, 0);
            var windowEnd = new DateTime(2015, 12, 31, 23, 59, 59);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._))
                .Returns(new QuarterWindow(windowStart, windowEnd));

            SystemTime.Freeze(timeNow, true);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new Quarter(2015, QuarterType.Q4));

            SystemTime.Unfreeze();

            Assert.Single(result);

            var error = result.Single();

            Assert.Equal(Core.Shared.ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("has closed", error.Description);
            Assert.Contains("2015", error.Description);
        }

        private SubmissionWindowClosed SubmissionWindowClosed()
        {
            return new SubmissionWindowClosed(quarterWindowFactory, systemDataDataAccess);
        }

        public class TestSystemData : SystemData
        {
        }
    }
}

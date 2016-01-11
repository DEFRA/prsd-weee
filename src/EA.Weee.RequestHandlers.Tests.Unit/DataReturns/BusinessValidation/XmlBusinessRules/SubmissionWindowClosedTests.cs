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
    using RequestHandlers.DataReturns.BusinessValidation.XmlBusinessRules;
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
        public async void UseFixedComplianceYearAndQuarter_AndComplianceYearAndQuarterMatch_NoValidationError()
        {
            var systemData = new TestSystemData();
            systemData.UpdateQuarterAndComplianceYear(new Quarter(2016, QuarterType.Q1));
            systemData.ToggleFixedQuarterAndComplianceYearUsage(true);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new SchemeReturn
                    {
                        ComplianceYear = "2016",
                        ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch
                    });

            Assert.Empty(result.ErrorData);
        }

        [Fact]
        public async void UseFixedComplianceYearAndQuarter_AndComplianceYearAndQuarterDoNotMatch_ValidationError_StatingFixedForTesting()
        {
            var systemData = new TestSystemData();
            systemData.UpdateQuarterAndComplianceYear(new Quarter(2016, QuarterType.Q1));
            systemData.ToggleFixedQuarterAndComplianceYearUsage(true);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new SchemeReturn
                    {
                        ComplianceYear = "2016",
                        ReturnPeriod = SchemeReturnReturnPeriod.Quarter2AprilJune
                    });

            Assert.Single(result.ErrorData);

            var error = result.ErrorData.Single();

            Assert.Equal(Core.Shared.ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("fixed for testing", error.Description);
        }

        [Fact]
        public async void NotUsingFixedComplianceYearAndQuarter_TimeNowIsWithinQuarterWindow_NoValidationError()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedQuarterAndComplianceYearUsage(false);

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
                    .Validate(new SchemeReturn
                    {
                        ComplianceYear = "2016",
                        ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch
                    });

            SystemTime.Unfreeze();

            Assert.Empty(result.ErrorData);
        }

        [Fact]
        public async void NotUsingFixedComplianceYearAndQuarter_TimeNowBeforeQuarterWindow_ValidationError_StatingWindowNotYetOpened()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedQuarterAndComplianceYearUsage(false);

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
                    .Validate(new SchemeReturn
                    {
                        ComplianceYear = "2016",
                        ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch
                    });

            SystemTime.Unfreeze();

            Assert.Single(result.ErrorData);

            var error = result.ErrorData.Single();

            Assert.Equal(Core.Shared.ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("not yet opened", error.Description);
        }

        [Fact]
        public async void NotUsingFixedComplianceYearAndQuarter_TimeAfterQuarterWindow_ValidationError_StatingWindowHasClosed()
        {
            var systemData = new TestSystemData();
            systemData.ToggleFixedQuarterAndComplianceYearUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 1, 3, 0, 0, 0);

            var windowStart = new DateTime(2016, 1, 1, 0, 0, 0);
            var windowEnd = new DateTime(2016, 1, 2, 0, 0, 0);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindow(A<Quarter>._))
                .Returns(new QuarterWindow(windowStart, windowEnd));

            SystemTime.Freeze(timeNow, true);

            var result = await
                SubmissionWindowClosed()
                    .Validate(new SchemeReturn
                    {
                        ComplianceYear = "2016",
                        ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch
                    });

            SystemTime.Unfreeze();

            Assert.Single(result.ErrorData);

            var error = result.ErrorData.Single();

            Assert.Equal(Core.Shared.ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("has closed", error.Description);
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

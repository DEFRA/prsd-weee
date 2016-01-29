namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using BusinessValidation.XmlBusinessRules;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Factories;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.DataReturns;
    using Requests.DataReturns;
    using Xunit;

    public class IsSubmissionWindowOpenHandlerTests
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public IsSubmissionWindowOpenHandlerTests()
        {
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
        }

        [Fact]
        public async void NotUsingFixedDate_TimeNowIsWithinOneOfTheQuarterWindow_ReturnTrue()
        {
            var systemData = new SubmissionWindowClosedTests.TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 4, 10, 0, 0, 0);

            var windowStart = new DateTime(2016, 4, 1, 0, 0, 0);
            var windowEnd = new DateTime(2017, 3, 16, 0, 0, 0);

            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._))
                .Returns(new List<QuarterWindow>
                {
                    new QuarterWindow(windowStart, windowEnd)
                });

            SystemTime.Freeze(timeNow, true);

            var handler = new IsSubmissionWindowOpenHandler(quarterWindowFactory, systemDataDataAccess);

            var result = await handler.HandleAsync(A.Dummy<IsSubmissionWindowOpen>());

            SystemTime.Unfreeze();

            Assert.True(result);
        }

        [Fact]
        public async void NotUsingFixedDate_TimeNowIsNotWithinOneOfTheQuarterWindow_ReturnFalse()
        {
            var systemData = new SubmissionWindowClosedTests.TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 3, 25, 0, 0, 0);
            
            A.CallTo(() => quarterWindowFactory.GetQuarterWindowsForDate(A<DateTime>._))
                .Returns(new List<QuarterWindow>());

            SystemTime.Freeze(timeNow, true);

            var handler = new IsSubmissionWindowOpenHandler(quarterWindowFactory, systemDataDataAccess);

            var result = await handler.HandleAsync(A.Dummy<IsSubmissionWindowOpen>());

            SystemTime.Unfreeze();

            Assert.False(result);
        }
    }
}

﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using DataAccess.DataAccess;
    using DataReturns.BusinessValidation.XmlBusinessRules;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.Shared;
    using Requests.Shared;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class GetApiUtcDateHandlerTests
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetApiUtcDateHandlerTests()
        {
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
        }

        [Fact]
        public async Task GetApiUtcDate_UseFixedCurrentDateIsTrue_ReturnFixedDate()
        {
            var systemData = new SubmissionWindowClosedTests.TestSystemData();
            var fixedDate = new DateTime(2018, 4, 10, 0, 0, 0);
            systemData.UpdateFixedCurrentDate(fixedDate);
            systemData.ToggleFixedCurrentDateUsage(true);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var handler = new GetApiUtcDateHandler(systemDataDataAccess);

            var result = await handler.HandleAsync(A.Dummy<GetApiUtcDate>());

            Assert.Equal(result, fixedDate);
        }

        [Fact]
        public async Task GetApiUtcDate_UseFixedCurrentDateIsFalse_ReturnCurrentDate()
        {
            var systemData = new SubmissionWindowClosedTests.TestSystemData();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get())
                .Returns(systemData);

            var timeNow = new DateTime(2016, 4, 10, 0, 0, 0);

            SystemTime.Freeze(timeNow, true);

            var handler = new GetApiUtcDateHandler(systemDataDataAccess);

            var result = await handler.HandleAsync(A.Dummy<GetApiUtcDate>());

            SystemTime.Unfreeze();

            Assert.Equal(result.ToUniversalTime().Date, timeNow.ToUniversalTime().Date);
        }
    }
}

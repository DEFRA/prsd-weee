namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns;
    using Weee.Tests.Core;
    using Xunit;

    public class DataReturnSubmissionsDataAccessTests
    {
        [Fact]
        public async Task GetPreviousSubmission_ThrowsArgumentException_IfSpecifiedDataReturnVersionIsNotSubmitted()
        {
            DataReturnSubmissionsDataAccess dataAccess =
                new DataReturnSubmissionsDataAccess(A.Dummy<WeeeContext>());

            var dataReturnVersion = A.Fake<DataReturnVersion>();
            A.CallTo(() => dataReturnVersion.IsSubmitted)
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => dataAccess.GetPreviousSubmission(dataReturnVersion));
        }

        [Fact]
        public async Task GetPreviousSubmission_ThrowsInvalidOperationException_IfSpecifiedDataReturnVersionIsNotSavedInDatabase()
        {
            var dbHelper = new DbContextHelper();

            var weeeContext = A.Fake<WeeeContext>();
            A.CallTo(() => weeeContext.DataReturnVersions)
                .Returns(dbHelper.GetAsyncEnabledDbSet(new List<DataReturnVersion>()));

            DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(weeeContext);

            var dataReturnVersion = A.Fake<DataReturnVersion>();
            A.CallTo(() => dataReturnVersion.IsSubmitted)
                .Returns(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetPreviousSubmission(dataReturnVersion));
        }
    }
}

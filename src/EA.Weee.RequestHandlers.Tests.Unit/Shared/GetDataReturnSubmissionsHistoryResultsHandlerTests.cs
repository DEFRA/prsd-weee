namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using FakeItEasy;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Shared;
    using Xunit;

    public class GetDataReturnSubmissionsHistoryResultsHandlerTests
    {
        [Fact]
        public async Task GetDataReturnSubmissionsHistoryResultHandler_RequestByExternalUser_ReturnDataReturnSubmissionsHistoryData()
        {
            // Arrange
            IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            GetDataReturnSubmissionsHistoryResultsHandler handler = new GetDataReturnSubmissionsHistoryResultsHandler(authorization, dataAccess);
            GetDataReturnSubmissionsHistoryResults request = new GetDataReturnSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());

            // Act
            List<DataReturnSubmissionsHistoryResult> results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(results.Count, 2);
        }

        private IGetDataReturnSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();

            var results = new List<DataReturnSubmissionsHistoryResult>()
            { 
                new DataReturnSubmissionsHistoryResult(),
                new DataReturnSubmissionsHistoryResult()
            };

            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._)).Returns(results);

            return dataAccess;
        }
    }
}

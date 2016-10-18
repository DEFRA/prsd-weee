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
            request.Ordering = DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending;

            // Act
            DataReturnSubmissionsHistoryResult results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(2, results.Data.Count);
            Assert.Equal(2, results.ResultCount);
        }

        [Fact]
        public async Task GetDataReturnSubmissionsHistoryResultHandler_RetrievesDataWithSpecifiedSort()
        {
            // Arrange
            var dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();
            var authorization = A.Fake<IWeeeAuthorization>();
            var handler = new GetDataReturnSubmissionsHistoryResultsHandler(authorization, dataAccess);

            var request = new GetDataReturnSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());
            request.Ordering = DataReturnSubmissionsHistoryOrderBy.QuarterDescending;

            // Act
            var results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, DataReturnSubmissionsHistoryOrderBy.QuarterDescending))
                .MustHaveHappened();
        }

        private IGetDataReturnSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();

            var results = new DataReturnSubmissionsHistoryResult
            {
                Data = new List<DataReturnSubmissionsHistoryData>
                {
                    new DataReturnSubmissionsHistoryData(),
                    new DataReturnSubmissionsHistoryData()
                },
                ResultCount = 2
            };

            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, A<DataReturnSubmissionsHistoryOrderBy>._))
                .Returns(results);

            return dataAccess;
        }
    }
}

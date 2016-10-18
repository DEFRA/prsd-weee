namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using FakeItEasy;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSubmissionsHistoryResultsHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Fact]
        public async Task GetSubmissionHistoryResultHandler_RequestByExternalUser_ReturnSubmissionHistoryData()
        {
            // Arrange
            IGetSubmissionsHistoryResultsDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);
            GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());
            request.Ordering = SubmissionsHistoryOrderBy.ComplianceYearAscending;

            // Act
            SubmissionsHistorySearchResult results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(2, results.Data.Count);
            Assert.Equal(2, results.ResultCount);
        }

        [Fact]
        public async Task GetDataReturnSubmissionsHistoryResultHandler_RetrievesDataWithSpecifiedSort()
        {
            // Arrange
            var dataAccess = A.Fake<IGetSubmissionsHistoryResultsDataAccess>();
            var authorization = A.Fake<IWeeeAuthorization>();
            var handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);

            var request = new GetSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());
            request.Ordering = SubmissionsHistoryOrderBy.SubmissionDateAscending;

            // Act
            var results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.GetSubmissionsHistory(A<Guid>._, A<int>._, SubmissionsHistoryOrderBy.SubmissionDateAscending))
                .MustHaveHappened();
        }

        private IGetSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetSubmissionsHistoryResultsDataAccess>();

            var results = new SubmissionsHistorySearchResult()
            {
                Data = new List<SubmissionsHistorySearchData>()
                {
                    new SubmissionsHistorySearchData(),
                    new SubmissionsHistorySearchData()
                },
                ResultCount = 2
            };

            A.CallTo(() => dataAccess.GetSubmissionsHistory(A<Guid>._, A<int>._, A<SubmissionsHistoryOrderBy>._))
                .Returns(results);

            return dataAccess;
        }
    }
}

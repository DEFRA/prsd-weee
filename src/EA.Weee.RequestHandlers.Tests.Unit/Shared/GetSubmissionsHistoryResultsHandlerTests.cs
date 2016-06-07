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

            // Act
            List<SubmissionsHistorySearchResult> results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(results.Count, 2);
        }

        private IGetSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetSubmissionsHistoryResultsDataAccess>();

            var results = new List<SubmissionsHistorySearchResult>()
            { 
                new SubmissionsHistorySearchResult(),
                new SubmissionsHistorySearchResult()
            };

            A.CallTo(() => dataAccess.GetSubmissionsHistory(A<Guid>._, A<int>._)).Returns(results);

            return dataAccess;
        }
    }
}

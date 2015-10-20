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
            // Arrage
            IGetSubmissionsHistoryResultsDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = A.Dummy<IWeeeAuthorization>();
            GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);
            GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(A<Guid>._);

            // Act
            List<SubmissionsHistorySearchResult> results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(results.Count, 2);
        }

        [Fact]
        public async Task GetSubmissionHistoryResultHandler_RequestByInternalUser_ReturnSubmissionHistoryDataByYear()
        {
            // Arrage
            IGetSubmissionsHistoryResultsDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = A.Dummy<IWeeeAuthorization>();
            GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);
            GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(A<Guid>._, 2016, A<Guid>._);

            // Act
            List<SubmissionsHistorySearchResult> results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureInternalOrOrganisationAccess(A<Guid>._)).MustHaveHappened();
            Assert.Equal(results.Count, 3);
        }

        private IGetSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetSubmissionsHistoryResultsDataAccess>();

            var results = new List<SubmissionsHistorySearchResult>()
            { 
                new SubmissionsHistorySearchResult(),
                new SubmissionsHistorySearchResult()
            };

            var resultsForYear = new List<SubmissionsHistorySearchResult>()
            { 
                new SubmissionsHistorySearchResult(),
                new SubmissionsHistorySearchResult(),
                new SubmissionsHistorySearchResult()
            };

            A.CallTo(() => dataAccess.GetSubmissionsHistory(A<Guid>._)).Returns(results);

            A.CallTo(() => dataAccess.GetSubmissionHistoryForComplianceYear(A<Guid>._, A<int>._)).Returns(resultsForYear);

            return dataAccess;
        }
    }
}

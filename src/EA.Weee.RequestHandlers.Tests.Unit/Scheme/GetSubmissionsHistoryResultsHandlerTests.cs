namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using FakeItEasy;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSubmissionsHistoryResultsHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Fact]
        public async Task GetSubmissionHistoryResultHandler_NoOrganisationUser_ThrowsSecurityException()
        {   
            // Arrange
            IGetSubmissionsHistoryResultsDataAccess dataAccess = A.Dummy<IGetSubmissionsHistoryResultsDataAccess>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);

            GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(Guid.NewGuid());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetSubmissionHistoryResultHandler_HappyPath_ReturnSubmissionHistoryData()
        {
            // Arrage
            IGetSubmissionsHistoryResultsDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .Build();
            GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);
            GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(A<Guid>._);

            // Act
            List<SubmissionsHistorySearchResult> results = await handler.HandleAsync(request);

            // Assert
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

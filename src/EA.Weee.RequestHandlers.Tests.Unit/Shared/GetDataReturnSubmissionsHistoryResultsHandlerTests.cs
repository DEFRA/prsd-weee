namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Shared;
    using Xunit;
    using QuarterType = Core.DataReturns.QuarterType;

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
            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, DataReturnSubmissionsHistoryOrderBy.QuarterDescending, A<bool>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetDataReturnSubmissionsHistoryResultHandler_RetrievesSummaryDataWhenSpecified()
        {
            // Arrange
            var dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();
            var authorization = A.Fake<IWeeeAuthorization>();
            var handler = new GetDataReturnSubmissionsHistoryResultsHandler(authorization, dataAccess);

            var request = new GetDataReturnSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());
            request.Ordering = DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending;
            request.IncludeSummaryData = true;

            // Act
            var results = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending, true))
                .MustHaveHappened();
        }

        [Fact]
        public async void GetDataReturnSubmissionsHistoryResultHandler_ReturnsSubmissionDataCount()
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
            Assert.Equal(2, results.ResultCount);
        }
        
        [Fact]
        public async void GetDataReturnSubmissionsHistoryResultHandler_MapsPropertiesToDataReturnSubmissionsHistoryResultObject()
        {
            // Arrange
            var dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();
            var authorization = A.Fake<IWeeeAuthorization>();
            var handler = new GetDataReturnSubmissionsHistoryResultsHandler(authorization, dataAccess);
            var request = new GetDataReturnSubmissionsHistoryResults(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<int>());
            request.Ordering = DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending;

            var schemeId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var complianceYear = 2016;
            var dataReturnUploadId = Guid.NewGuid();
            var fileName = "TestFileName";
            var quarter = QuarterType.Q3;
            var submittedBy = "TestUser";
            var submissionDateTime = new DateTime(2016, 1, 1);

            var dataReturnVersionId = Guid.NewGuid();
            var dataReturnVersion = A.Fake<DataReturnVersion>();
            A.CallTo(() => dataReturnVersion.Id)
                .Returns(dataReturnVersionId);

            var eeeOutputB2b = 1;
            var eeeOutputB2c = 2;

            var weeeCollectedB2b = 3;
            var weeeCollectedB2c = 4;

            var weeeDeliveredB2b = 5;
            var weeeDeliveredB2c = 6;

            var data = new DataReturnSubmissionsData
            {
                SchemeId = schemeId,
                OrganisationId = organisationId,
                ComplianceYear = complianceYear,
                DataReturnUploadId = dataReturnUploadId,
                FileName = fileName,
                Quarter = quarter,
                SubmittedBy = submittedBy,
                SubmissionDateTime = submissionDateTime,
                DataReturnVersion = dataReturnVersion,
                EeeOutputB2b = eeeOutputB2b,
                EeeOutputB2c = eeeOutputB2c,
                WeeeCollectedB2b = weeeCollectedB2b,
                WeeeCollectedB2c = weeeCollectedB2c,
                WeeeDeliveredB2b = weeeDeliveredB2b,
                WeeeDeliveredB2c = weeeDeliveredB2c
            };

            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, A<DataReturnSubmissionsHistoryOrderBy>._, A<bool>._))
                .Returns(new List<DataReturnSubmissionsData> { data });

            // Act
            DataReturnSubmissionsHistoryResult results = await handler.HandleAsync(request);

            // Assert
            Assert.Single(results.Data);

            var result = results.Data[0];

            Assert.Equal(schemeId, result.SchemeId);
            Assert.Equal(organisationId, result.OrganisationId);
            Assert.Equal(complianceYear, result.ComplianceYear);
            Assert.Equal(dataReturnUploadId, result.DataReturnUploadId);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal(quarter, result.Quarter);
            Assert.Equal(submissionDateTime, result.SubmissionDateTime);
            Assert.Equal(dataReturnVersionId, result.DataReturnVersionId);
            Assert.Equal(eeeOutputB2b, result.EeeOutputB2b);
            Assert.Equal(eeeOutputB2c, result.EeeOutputB2c);
            Assert.Equal(weeeCollectedB2b, result.WeeeCollectedB2b);
            Assert.Equal(weeeCollectedB2c, result.WeeeCollectedB2c);
            Assert.Equal(weeeDeliveredB2b, result.WeeeDeliveredB2b);
            Assert.Equal(weeeDeliveredB2c, result.WeeeDeliveredB2c);
        }

        private IGetDataReturnSubmissionsHistoryResultsDataAccess CreateFakeDataAccess()
        {
            IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = A.Fake<IGetDataReturnSubmissionsHistoryResultsDataAccess>();

            var results = new List<DataReturnSubmissionsData>
            {
                new DataReturnSubmissionsData() { DataReturnVersion = A.Dummy<DataReturnVersion>() },
                new DataReturnSubmissionsData() { DataReturnVersion = A.Dummy<DataReturnVersion>() }
            };

            A.CallTo(() => dataAccess.GetDataReturnSubmissionsHistory(A<Guid>._, A<int>._, A<DataReturnSubmissionsHistoryOrderBy>._, A<bool>._))
                .Returns(results);

            return dataAccess;
        }
    }
}

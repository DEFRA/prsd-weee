namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using FakeItEasy;
    using RequestHandlers.Admin.GetDataReturnSubmissionChanges;
    using Requests.Admin.GetDataReturnSubmissionChanges;
    using Weee.Tests.Core;
    using Xunit;

    public class GetDataReturnSubmissionEeeChangesCsvHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void HandleAsync_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);

            var handler = new GetDataReturnSubmissionEeeChangesCsvHandler(
                authorization,
                A.Dummy<IGetDataReturnSubmissionEeeChangesCsvDataAccess>(),
                A.Dummy<ICsvWriter<DataReturnSubmissionEeeChangesCsvData>>());

            // Act
            Func<Task<CSVFileData>> action = () => handler.HandleAsync(A.Dummy<GetDataReturnSubmissionEeeChangesCsv>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetDataReturnSubmissionEeeChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<DataReturnSubmissionEeeChangesCsvData>>();

            var handler = new GetDataReturnSubmissionEeeChangesCsvHandler(authorization, dataAccess, csvWriter);

            var currentSubmissionId = Guid.NewGuid();
            var previousSubmissionId = Guid.NewGuid();

            var request = new GetDataReturnSubmissionEeeChangesCsv(currentSubmissionId, previousSubmissionId);

            A.CallTo(() => csvWriter.Write(A<IEnumerable<DataReturnSubmissionEeeChangesCsvData>>._))
                .Returns("CsvData");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.GetChanges(currentSubmissionId, previousSubmissionId))
                .MustHaveHappened();

            A.CallTo(() => csvWriter.Write(A<IEnumerable<DataReturnSubmissionEeeChangesCsvData>>._))
                .MustHaveHappened();

            Assert.NotEmpty(result.FileContent);
            Assert.Equal("CsvData", result.FileContent);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileName()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetDataReturnSubmissionEeeChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<DataReturnSubmissionEeeChangesCsvData>>();

            var handler = new GetDataReturnSubmissionEeeChangesCsvHandler(authorization, dataAccess, csvWriter);

            A.CallTo(() => dataAccess.GetChanges(A<Guid>._, A<Guid>._))
                .Returns(new DataReturnSubmissionEeeChanges
                {
                    SchemeApprovalNumber = "ABC",
                    ComplianceYear = 2016,
                    Quarter = 3,
                    CurrentSubmissionDate = new DateTime(2016, 2, 1, 4, 5, 0)
                });

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetDataReturnSubmissionEeeChangesCsv>());

            // Assert
            Assert.Equal("2016_Q3_ABC_EEEDataChanges_01022016_0405.csv", result.FileName);
        }
    }
}
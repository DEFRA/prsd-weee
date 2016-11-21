namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Admin.GetSubmissionChanges;
    using Requests.Admin.GetSubmissionChanges;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSubmissionChangesCsvHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);

            var handler = new GetSubmissionChangesCsvHandler(
                authorization,
                A.Dummy<IGetSubmissionChangesCsvDataAccess>(),
                A.Dummy<ICsvWriter<SubmissionChangesCsvData>>());

            // Act
            Func<Task<CSVFileData>> action = () => handler.HandleAsync(A.Dummy<GetSubmissionChangesCsv>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ThrowsInvalidOperationExceptionIfMemberUploadNotFound()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetSubmissionChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<SubmissionChangesCsvData>>();

            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .Returns((MemberUpload)null);

            var handler = new GetSubmissionChangesCsvHandler(authorization, dataAccess, csvWriter);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(A.Dummy<GetSubmissionChangesCsv>()));

            // Assert
            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .MustHaveHappened();

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task HandleAsync_ThrowsInvalidOperationExceptionIfMemberUploadIsNotSubmitted()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetSubmissionChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<SubmissionChangesCsvData>>();

            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.IsSubmitted)
                .Returns(false);

            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .Returns(memberUpload);

            var handler = new GetSubmissionChangesCsvHandler(authorization, dataAccess, csvWriter);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(A.Dummy<GetSubmissionChangesCsv>()));

            // Assert
            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .MustHaveHappened();

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetSubmissionChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<SubmissionChangesCsvData>>();

            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.IsSubmitted)
                .Returns(true);

            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .Returns(memberUpload);

            var handler = new GetSubmissionChangesCsvHandler(authorization, dataAccess, csvWriter);

            var submissionId = Guid.NewGuid();
            var request = new GetSubmissionChangesCsv(submissionId);

            A.CallTo(() => csvWriter.Write(A<IEnumerable<SubmissionChangesCsvData>>._))
                .Returns("CsvData");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.GetSubmissionChanges(submissionId))
                .MustHaveHappened();

            A.CallTo(() => csvWriter.Write(A<IEnumerable<SubmissionChangesCsvData>>._))
                .MustHaveHappened();

            Assert.NotEmpty(result.FileContent);
            Assert.Equal("CsvData", result.FileContent);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileName()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IGetSubmissionChangesCsvDataAccess>();
            var csvWriter = A.Fake<ICsvWriter<SubmissionChangesCsvData>>();

            var handler = new GetSubmissionChangesCsvHandler(authorization, dataAccess, csvWriter);

            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.ComplianceYear)
                .Returns(2016);

            A.CallTo(() => memberUpload.IsSubmitted)
                .Returns(true);

            A.CallTo(() => memberUpload.SubmittedDate)
                .Returns(new DateTime(2016, 2, 1, 4, 5, 0));

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("ABC");

            A.CallTo(() => memberUpload.Scheme)
                .Returns(scheme);

            A.CallTo(() => dataAccess.GetMemberUpload(A<Guid>._))
                .Returns(memberUpload);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetSubmissionChangesCsv>());

            // Assert
            Assert.Equal("2016_ABC_memberchanges_01022016_0405.csv", result.FileName);
        }
    }
}
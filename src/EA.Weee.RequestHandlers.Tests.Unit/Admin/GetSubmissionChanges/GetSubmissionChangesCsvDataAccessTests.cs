namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using RequestHandlers.Admin.GetSubmissionChanges;
    using Xunit;

    public class GetSubmissionChangesCsvDataAccessTests
    {
        [Fact]
        public async Task GetSubmissionChanges_InvokesStoredProcedureAndReturnsResult()
        {
            // Arrange
            var context = A.Fake<WeeeContext>();
            var memberUploadId = Guid.NewGuid();

            var storedProcedureResult = new List<SubmissionChangesCsvData>();

            A.CallTo(() => context.StoredProcedures.SpgSubmissionChangesCsvData(memberUploadId))
                .Returns(storedProcedureResult);

            var dataAccess = new GetSubmissionChangesCsvDataAccess(context);

            // Act
            var result = await dataAccess.GetSubmissionChanges(memberUploadId);

            // Assert
            A.CallTo(() => context.StoredProcedures.SpgSubmissionChangesCsvData(memberUploadId))
                .MustHaveHappened();

            Assert.Equal(storedProcedureResult, result);
        }
    }
}

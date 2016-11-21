namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetSubmissionChanges
{
    using System;
    using System.Threading.Tasks;
    using RequestHandlers.Admin.GetSubmissionChanges;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSubmissionChangesCsvDataAccessTests
    {
        [Fact]
        public async Task GetMemberUpload_ReturnsMemberUploadWithSpecifiedId()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                db.Model.SaveChanges();

                var dataAccess = new GetSubmissionChangesCsvDataAccess(db.WeeeContext);

                // Act
                var result = await dataAccess.GetMemberUpload(memberUpload.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(memberUpload.Id, result.Id); 
            }
        }

        [Fact]
        public async Task GetMemberUpload_ReturnsNull_WhenMemberUploadWithSpecifiedIdNotFound()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                db.Model.SaveChanges();

                var dataAccess = new GetSubmissionChangesCsvDataAccess(db.WeeeContext);

                // Act
                var result = await dataAccess.GetMemberUpload(Guid.NewGuid());

                // Assert
                Assert.Null(result);
            }
        }
    }
}

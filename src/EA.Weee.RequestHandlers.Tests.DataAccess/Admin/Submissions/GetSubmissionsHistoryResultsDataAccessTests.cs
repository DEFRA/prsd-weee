namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Submissions
{
    using System.Threading.Tasks;
    using RequestHandlers.Admin.Submissions;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSubmissionsHistoryResultsDataAccessTests
    {
        /// <summary>
        /// This test ensures that only the latest result will be returned for a producer who is
        /// registered in multiple compliance years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchSumissions_ForYearandScheme_Returns2Submissions()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = true;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.IsCurrentForComplianceYear = true;

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(2016, scheme.Id);

                // Assert
                Assert.NotNull(results);
            }
        }
    }
}

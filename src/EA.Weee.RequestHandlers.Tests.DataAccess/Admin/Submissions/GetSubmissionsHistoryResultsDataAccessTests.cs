namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Submissions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using RequestHandlers.Shared;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSubmissionsHistoryResultsDataAccessTests
    {
        /// <summary>
        /// This test ensures that only the submissions for the selected year and scheme
        /// registered in multiple compliance years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchSumissions_ForYearandScheme_ReturnsSubmittedSubmissions()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme1 = helper.CreateScheme();
                Scheme scheme2 = helper.CreateScheme();

                var user1 = helper.CreateUser("Test@co.uk");
                var user2 = helper.CreateUser("a@co.uk");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2005;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 09, 22, 10, 45, 45);
                memberUpload1.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2006;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 08, 10, 12, 25, 35);
                memberUpload2.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2006;
                memberUpload3.IsSubmitted = false;
                memberUpload3.SubmittedDate = new DateTime(2015, 09, 10, 12, 25, 35);
                memberUpload3.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload4 = helper.CreateMemberUpload(scheme2);
                memberUpload4.ComplianceYear = 2006;
                memberUpload4.IsSubmitted = true;
                memberUpload4.SubmittedDate = new DateTime(2015, 07, 22, 10, 45, 45);
                memberUpload4.SubmittedByUserId = user2.Id;

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess =
                    new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(scheme1.Id, 2006);

                // Assert
                Assert.NotNull(results.Data);
                Assert.Equal(1, results.Data.Count);

                var result1 = results.Data.SingleOrDefault(r => r.Year == 2006);
                Assert.NotNull(result1);
                Assert.Equal("08/10/2015 12:25:35", result1.DateTime.ToString(CultureInfo.InvariantCulture));
                Assert.Equal("Test LastName", result1.SubmittedBy);
            }
        }

        [Fact]
        public async Task FetchSumissions_ForYearandScheme_ReturnsSubmittedSubmissionsWithCorrectnumberOfWarnings()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme1 = helper.CreateScheme();
                Scheme scheme2 = helper.CreateScheme();

                var user1 = helper.CreateUser("Test1@co.uk");
                var user2 = helper.CreateUser("a1@co.uk");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2006;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 09, 23, 10, 45, 45);
                memberUpload1.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2006;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 08, 4, 12, 24, 35);
                memberUpload2.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2006;
                memberUpload3.IsSubmitted = false;
                memberUpload3.SubmittedDate = new DateTime(2015, 08, 10, 12, 25, 32);
                memberUpload3.SubmittedByUserId = user1.Id;

                MemberUpload memberUpload4 = helper.CreateMemberUpload(scheme2);
                memberUpload4.ComplianceYear = 2006;
                memberUpload4.IsSubmitted = true;
                memberUpload4.SubmittedDate = new DateTime(2015, 07, 31, 10, 25, 45);
                memberUpload4.SubmittedByUserId = user2.Id;

                helper.CreateMemberUploadError(memberUpload2);
                helper.CreateMemberUploadError(memberUpload2);

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(scheme1.Id, 2006);

                // Assert
                Assert.NotNull(results.Data);
                Assert.Equal(2, results.Data.Count);
                Assert.Collection(results.Data,
                    r1 => Assert.Equal(0, r1.NoOfWarnings),
                    r2 => Assert.Equal(2, r2.NoOfWarnings));
            }
        }
    }
}

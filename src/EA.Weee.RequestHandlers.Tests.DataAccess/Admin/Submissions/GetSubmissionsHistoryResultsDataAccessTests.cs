namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Submissions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using RequestHandlers.Admin.Submissions;
    using Weee.Tests.Core;
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
        public async Task FetchSumissions_ForYearandScheme_ReturnsCorrectSubmissions()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                AspNetUser user = new AspNetUser();
                user.Id = Guid.NewGuid().ToString();
                user.FirstName = "Test";
                user.Surname = "LastName";
                user.Email = "test@co.uk";
                user.EmailConfirmed = true;
                user.UserName = "test@co.uk";
                database.Model.AspNetUsers.Add(user);

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;
                memberUpload1.Date = new DateTime(2015, 09, 22, 10, 45, 45);
                
                memberUpload1.UserId = user.Id;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = true;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.Date = new DateTime(2015, 08, 10, 12, 25, 35);
                memberUpload2.UserId = user.Id;

                Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.IsCurrentForComplianceYear = true;

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(2016, scheme.Id);
                
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                
                var result1 = results.SingleOrDefault(r => r.Year == 2016);
                Assert.NotNull(result1);
                Assert.Equal("08/10/2015 12:25:35", result1.DateTime.ToString(CultureInfo.InvariantCulture));
                Assert.Equal("Test LastName", result1.SubmittedBy);
            }
        }
    }
}

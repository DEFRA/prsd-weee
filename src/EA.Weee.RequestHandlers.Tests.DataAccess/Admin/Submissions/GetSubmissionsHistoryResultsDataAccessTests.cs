﻿namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Submissions
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
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;
                memberUpload1.Date = new DateTime(2015, 09, 22, 10, 45, 45);
                memberUpload1.UserId = user1.Id;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.Date = new DateTime(2015, 08, 10, 12, 25, 35);
                memberUpload2.UserId = user1.Id;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.IsSubmitted = false;
                memberUpload3.Date = new DateTime(2015, 09, 10, 12, 25, 35);
                memberUpload3.UserId = user1.Id;

                MemberUpload memberUpload4 = helper.CreateMemberUpload(scheme2);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.IsSubmitted = true;
                memberUpload4.Date = new DateTime(2015, 07, 22, 10, 45, 45);
                memberUpload4.UserId = user2.Id;

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess =
                    new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(2016, scheme1.Id);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                var result1 = results.SingleOrDefault(r => r.Year == 2016);
                Assert.NotNull(result1);
                Assert.Equal("08/10/2015 12:25:35", result1.DateTime.ToString(CultureInfo.InvariantCulture));
                Assert.Equal("Test LastName", result1.SubmittedBy);
            }
        }

        [Fact]
        public async Task FetchSumissions_ForYearandScheme_ReturnsSubmittedSubmissionsWithWarnings()
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
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;
                memberUpload1.Date = new DateTime(2015, 09, 23, 10, 45, 45);
                memberUpload1.UserId = user1.Id;
                
                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.Date = new DateTime(2015, 08, 4, 12, 24, 35);
                memberUpload2.UserId = user1.Id;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.IsSubmitted = false;
                memberUpload3.Date = new DateTime(2015, 08, 10, 12, 25, 32);
                memberUpload3.UserId = user1.Id;

                MemberUpload memberUpload4 = helper.CreateMemberUpload(scheme2);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.IsSubmitted = true;
                memberUpload4.Date = new DateTime(2015, 07, 31, 10, 25, 45);
                memberUpload4.UserId = user2.Id;

                helper.CreateMemberUploadError(memberUpload2);
                helper.CreateMemberUploadError(memberUpload2);

                database.Model.SaveChanges();

                // Act
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);
                var results = await dataAccess.GetSubmissionsHistory(2016, scheme1.Id);
                
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                
                var result1 = results.SingleOrDefault(r => r.Year == 2016);
                Assert.NotNull(result1);
                Assert.Equal(2, result1.NoOfWarnings);
            }
        }
    }
}

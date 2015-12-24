namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using System;
    using RequestHandlers.Shared;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSubmissionHistoryDataAcessTests
    {
        [Fact]
        public async void GetSubmissionHistory_WitValidSchemeId_ReturnsAllYearsSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var user1 = modelHelper.CreateUser("Test");
                database.Model.SaveChanges();

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.OrganisationId = organisationId;
                mu1.CreatedDate = DateTime.Now;
                mu1.CreatedById = user1.Id;

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2015;
                mu2.IsSubmitted = true;
                mu2.OrganisationId = organisationId;
                mu2.CreatedDate = DateTime.Now;
                mu2.CreatedById = user1.Id;

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.OrganisationId = organisationId;
                mu3.CreatedDate = DateTime.Now;
                mu3.CreatedById = user1.Id;

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 3);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_WitValidSchemeIdAndYear2015_ReturnsOnly2015YearSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var user1 = modelHelper.CreateUser("Test");
                database.Model.SaveChanges();

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.OrganisationId = organisationId;
                mu1.CreatedDate = DateTime.Now;
                mu1.CreatedById = user1.Id;

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2015;
                mu2.IsSubmitted = true;
                mu2.OrganisationId = organisationId;
                mu2.CreatedDate = DateTime.Now;
                mu2.CreatedById = user1.Id;

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.OrganisationId = organisationId;
                mu3.CreatedDate = DateTime.Now;
                mu3.CreatedById = user1.Id;

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, 2015);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 2);
                Assert.Collection(result,
                                  r1 => Assert.Equal(r1.Year, 2015),
                                  r2 => Assert.Equal(r2.Year, 2015));
            }
        }
    }
}

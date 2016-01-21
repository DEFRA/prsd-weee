namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using System;
    using Core.DataReturns;
    using RequestHandlers.Shared;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetDataReturnSubmissionHistoryDataAcessTests
    {
        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsAllSubmittedSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, false);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme1, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2016, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2016;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 2);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSpecifiedSchemeSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation.Id = anotherOrganisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme2, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme2, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2015;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 2);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSchemeSubmissionHistoryDataOrderByDescendingSubmissionDateTime()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";
                dru1.DataReturnVersion.SubmittedDate = DateTime.Today;

                var drv2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme1, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";
                dru2.DataReturnVersion.SubmittedDate = DateTime.Today.AddDays(1);

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2015;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";
                dru3.DataReturnVersion.SubmittedDate = DateTime.Today.AddDays(2);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 3);
                Assert.Equal(result[0].FileName, "DataReturnUpload3.xml");
                Assert.Equal(result[1].FileName, "DataReturnUpload2.xml");
                Assert.Equal(result[2].FileName, "DataReturnUpload1.xml");
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSpecifiedSchemeAndComplianceYearSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation.Id = anotherOrganisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme2, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme2, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2016, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2016;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id, 2015);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 1);
                Assert.Equal(result[0].FileName, "DataReturnUpload1.xml");
            }
        }
    }
}

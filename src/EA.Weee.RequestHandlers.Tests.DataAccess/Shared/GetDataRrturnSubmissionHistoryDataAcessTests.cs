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
        public async void GetDataReturnSubmissionHistory_WitValidSchemeId_ReturnsAllYearsSubmissionHistoryData()
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

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 1);
            }
        }
    }
}

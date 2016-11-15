namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;
    public class GetSchemeObligationDataCsvHandlerTests
    {
        [Fact]
        public async void DataAccess_ReturnsOnlyB2CSchemeForProducer_WhenTheyAreRegisteredB2BandB2COverTwoSchemesInCurrentYear()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE0001ST/SCH";
                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE0002ST/SCH";

                //Previous Year Data
                //This test is registering the producer over two schemes in the previous year as well as the current
                //year to test the additional complexity
                var memberUpload2000s1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload2000s1.ComplianceYear = 2000;
                var prod1_2000s1 = helper.CreateProducerAsCompany(memberUpload2000s1, "PRN123", "B2B");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000s1.RegisteredProducer, "B2B", 1, 500);

                var memberUpload2000s2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2000s2.ComplianceYear = 2000;
                var prod1_2000s2 = helper.CreateProducerAsCompany(memberUpload2000s2, "PRN123", "B2C");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2000s2.RegisteredProducer, "B2C", 1, 70);

                //Current Year Data
                var memberUpload2001s1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload2001s1.ComplianceYear = 2001;
                var prod1_2001s1 = helper.CreateProducerAsCompany(memberUpload2001s1, "PRN123", "B2C");
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion3, prod1_2001s1.RegisteredProducer, "B2C", 1, 5000);

                var memberUpload2001s2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2001s2.ComplianceYear = 2001;
                helper.CreateProducerAsCompany(memberUpload2001s2, "PRN123", "B2B");

                db.Model.SaveChanges();

                // Act
                GetSchemeObligationCsvDataProcessor obligationDataAccess = new GetSchemeObligationCsvDataProcessor(db.WeeeContext);
                var results = await obligationDataAccess.FetchObligationsForComplianceYearAsync(2001);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result1 = results.Find(x => (x.ApprovalNumber == "WEE/TE0001ST/SCH"));
                Assert.Equal("PRN123", result1.PRN);
                Assert.Equal("B2C", result1.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result1.ObligationTypeForSelectedYear);
                Assert.Equal(70, result1.Cat1B2CTotal);
            }
        }
    }
}

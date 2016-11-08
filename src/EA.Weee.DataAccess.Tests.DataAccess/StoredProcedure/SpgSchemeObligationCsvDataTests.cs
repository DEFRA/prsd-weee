namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgSchemeObligationCsvDataTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerEeeForPreviousYear()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2C");
                var prod2_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN456", "B2B");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2000.RegisteredProducer, "B2C", 2, 1000);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod2_2000.RegisteredProducer, "B2B", 2, 400);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2C");
                var prod2_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN456", "B2B");
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, 2001, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion3, prod1_2001.RegisteredProducer, "B2C", 1, 3000);
                helper.CreateEeeOutputAmount(dataReturnVersion4, prod1_2001.RegisteredProducer, "B2C", 2, 600);
                helper.CreateEeeOutputAmount(dataReturnVersion3, prod2_2001.RegisteredProducer, "B2B", 2, 9000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                //Producer with only B2B (both years) should not be in data
                SchemeObligationCsvData b2bProducer = results.Find(x => (x.PRN == "PRN456"));
                Assert.Null(b2bProducer);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("test scheme name", result.SchemeName);
                Assert.Equal("WEE/TE0000ST/SCH", result.ApprovalNumber);
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal(100, result.Cat1B2CTotal);
                Assert.Equal(1000, result.Cat2B2CTotal);
                Assert.Null(result.Cat3B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearB2CProducers_WhenTheyWerePreviousYearB2B_WithoutEeeData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2B");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2B", 1, 100);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2C");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2C", 1, 3000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2B", result.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result.ObligationTypeForSelectedYear);
                Assert.Null(result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearBothProducers_WhenTheyWerePreviousYearB2B_WithoutEeeData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2B");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2B", 1, 100);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "Both");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2C", 1, 3000);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2B", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2B", result.ObligationTypeForPreviousYear);
                Assert.Equal("Both", result.ObligationTypeForSelectedYear);
                Assert.Null(result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_DoesNotReturnCurrentYearB2BProducers_WhenTheyWerePreviousYearB2B()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2B");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2B", 1, 100);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2B");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2B", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearB2BProducers_WhenTheyWerePreviousYearB2C()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2C");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 1, 100);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2B");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2B", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2C", result.ObligationTypeForPreviousYear);
                Assert.Equal("B2B", result.ObligationTypeForSelectedYear);
                Assert.Equal(100, result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearB2BProducers_WhenTheyWerePreviousYearBoth()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Previous Year Data
                var memberUpload2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2000.ComplianceYear = 2000;
                var prod1_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "Both");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2B", 1, 500);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2B");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2B", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("Both", result.ObligationTypeForPreviousYear);
                Assert.Equal("B2B", result.ObligationTypeForSelectedYear);
                Assert.Equal(100, result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsSingleB2CResultForProducer_WhenTheyWerePreviousYearRegisteredB2BandB2COverTwoSchemes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE0001ST/SCH";
                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE0002ST/SCH";

                //Previous Year Data
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
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2C");
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion3, prod1_2001.RegisteredProducer, "B2C", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2C", result.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result.ObligationTypeForSelectedYear);
                Assert.Equal(70, result.Cat1B2CTotal);
            }
        }
    }
}

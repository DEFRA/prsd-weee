namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme.MemberUploadTesting;
    using System;
    using System.Linq;
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

                Assert.Single(results);
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
        public async Task Execute_ReturnsProducerEeeData_For_All_Categories()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                string prn1 = "PRN123";
                string prn2 = "PRN456";
                int complianceYear = DateTime.Now.Year;
                string approvalNumber = "WEE/TE0000ST/SCH";
                string b2bObligationType = EnumHelper.GetDisplayName(ObligationType.B2B);
                string b2cObligationType = EnumHelper.GetDisplayName(ObligationType.B2C);

                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = approvalNumber;

                var categories = EnumHelper.GetValues(typeof(WeeeCategory));
                var maxCategoryId = categories.Max(x => x.Key);
                int quarter1Tonnage = 0;
                int quarter2Tonnage = 0;

                //Previous Year Data
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload1.ComplianceYear = complianceYear - 1;
                var prod1 = helper.CreateProducerAsCompany(memberUpload1, prn1, b2cObligationType);
                var prod2 = helper.CreateProducerAsCompany(memberUpload1, prn2, b2bObligationType);
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, complianceYear - 1, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, complianceYear - 1, 2);
                for (int categoryId = 1; categoryId <= maxCategoryId; categoryId++)
                {
                    quarter1Tonnage = quarter1Tonnage + 10;
                    quarter2Tonnage = quarter2Tonnage + 5;
                    helper.CreateEeeOutputAmount(dataReturnVersion1, prod1.RegisteredProducer, b2cObligationType, categoryId, quarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion2, prod1.RegisteredProducer, b2cObligationType, categoryId, quarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion1, prod2.RegisteredProducer, b2bObligationType, categoryId, quarter2Tonnage);
                }

                //Current Year Data
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = complianceYear;
                var prod3 = helper.CreateProducerAsCompany(memberUpload2, prn1, b2cObligationType);
                var prod4 = helper.CreateProducerAsCompany(memberUpload2, prn2, b2bObligationType);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, complianceYear, 2);
                for (int categoryId = 1; categoryId <= maxCategoryId; categoryId++)
                {
                    quarter1Tonnage = quarter1Tonnage + 100;
                    quarter2Tonnage = quarter2Tonnage + 50;
                    helper.CreateEeeOutputAmount(dataReturnVersion3, prod3.RegisteredProducer, b2cObligationType, categoryId, quarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion4, prod3.RegisteredProducer, b2cObligationType, categoryId, quarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion3, prod4.RegisteredProducer, b2bObligationType, categoryId, quarter2Tonnage);
                }

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(complianceYear);

                //Assert
                Assert.NotNull(results);

                //Producer with only B2B (both years) should not be in data
                SchemeObligationCsvData b2bProducer = results.Find(x => (x.PRN == prn2));
                Assert.Null(b2bProducer);

                Assert.Single(results);
                SchemeObligationCsvData result = results[0];
                Assert.Equal(approvalNumber, result.ApprovalNumber);
                Assert.Equal(prn1, result.PRN);
                Assert.Equal(15, result.Cat1B2CTotal);
                Assert.Equal(30, result.Cat2B2CTotal);
                Assert.Equal(45, result.Cat3B2CTotal);
                Assert.Equal(60, result.Cat4B2CTotal);
                Assert.Equal(75, result.Cat5B2CTotal);
                Assert.Equal(90, result.Cat6B2CTotal);
                Assert.Equal(105, result.Cat7B2CTotal);
                Assert.Equal(120, result.Cat8B2CTotal);
                Assert.Equal(135, result.Cat9B2CTotal);
                Assert.Equal(150, result.Cat10B2CTotal);
                Assert.Equal(165, result.Cat11B2CTotal);
                Assert.Equal(180, result.Cat12B2CTotal);
                Assert.Equal(195, result.Cat13B2CTotal);
                Assert.Equal(210, result.Cat14B2CTotal);
                Assert.Equal(225, result.Cat15B2CTotal);
            }
        }

        /// <summary>
        /// Adds data for multiple producers, over multiple years, quarters, obligation types and categories.
        /// This checks that the data is returned correctly, using only B2C obligation values for the previous year,
        /// split by category and producer and totalled across all quarters for that year.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_ReturnsCorrectTotalsForCategories_OfB2COnlyForPreviousYearOfCorrectProducer()
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
                var prod2_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN456", "B2C");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 1, 100); // Prod1 Cat1 B2C Q1
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 2, 500); // Prod1 Cat2 B2C Q1
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2000.RegisteredProducer, "B2C", 2, 1000); // Prod1 Cat2 B2C Q2
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2B", 2, 50); // Prod1 Cat2 B2B Q1
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod2_2000.RegisteredProducer, "B2C", 2, 400); // Prod2 Cat2 B2C Q1

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
                Assert.Equal(2, results.Count);

                SchemeObligationCsvData firstProducer = results.Find(x => (x.PRN == "PRN123"));

                Assert.Equal("PRN123", firstProducer.PRN);
                Assert.Equal(100, firstProducer.Cat1B2CTotal);
                Assert.Equal(1500, firstProducer.Cat2B2CTotal);
                Assert.Null(firstProducer.Cat3B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsPreviousYearDataFromOldScheme_WhenProducerRegisteredWithNewScheme()
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
                var prod1_2000s1 = helper.CreateProducerAsCompany(memberUpload2000s1, "PRN123", "B2C");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000s1.RegisteredProducer, "B2C", 1, 500);

                //Current Year Data
                var memberUpload2001s2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2001s2.ComplianceYear = 2001;
                var prod1_2001s2 = helper.CreateProducerAsCompany(memberUpload2001s2, "PRN123", "B2C");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme2, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001s2.RegisteredProducer, "B2C", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);
                Assert.Single(results);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("test scheme name", result.SchemeName);
                Assert.Equal("WEE/TE0002ST/SCH", result.ApprovalNumber);
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal(500, result.Cat1B2CTotal);
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

                Assert.Single(results);
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

                Assert.Single(results);
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
                Assert.Empty(results);
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

                Assert.Single(results);
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

                Assert.Single(results);
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

                Assert.Single(results);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2C", result.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result.ObligationTypeForSelectedYear);
                Assert.Equal(70, result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsTwoResultsForProducer_WhenTheyAreRegisteredB2BandB2COverTwoSchemesInCurrentYear()
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
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.Count);
                SchemeObligationCsvData result1 = results.Find(x => (x.ApprovalNumber == "WEE/TE0001ST/SCH"));
                Assert.Equal("PRN123", result1.PRN);
                Assert.Equal("B2C", result1.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result1.ObligationTypeForSelectedYear);
                Assert.Equal(70, result1.Cat1B2CTotal);

                SchemeObligationCsvData result2 = results.Find(x => (x.ApprovalNumber == "WEE/TE0002ST/SCH"));
                Assert.Equal("PRN123", result2.PRN);
                Assert.Equal("B2C", result2.ObligationTypeForPreviousYear);
                Assert.Equal("B2B", result2.ObligationTypeForSelectedYear);
                Assert.Equal(70, result2.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearProducers_WhenTheyWereNotRegisteredInPreviousYear_WithoutEeeData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2C");
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2001.RegisteredProducer, "B2C", 1, 5000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);
                Assert.Single(results);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Null(result.ObligationTypeForPreviousYear);
                Assert.Equal("B2C", result.ObligationTypeForSelectedYear);
                Assert.Null(result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_ReturnsCurrentYearProducers_WhenNoPreviousYearDataReturnUpload_WithoutEeeData()
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
                helper.CreateProducerAsCompany(memberUpload2000, "PRN123", "B2C");

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
                Assert.Single(results);
                SchemeObligationCsvData result = results[0];
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal("B2C", result.ObligationTypeForPreviousYear);
                Assert.Equal("Both", result.ObligationTypeForSelectedYear);
                Assert.Null(result.Cat1B2CTotal);
            }
        }

        [Fact]
        public async Task Execute_DoesNotReturnUnregisteredCurrentYearProducers_WhenTheyHavePreviousYearData()
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
                var prod2_2000 = helper.CreateProducerAsCompany(memberUpload2000, "PRN456", "B2C");
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod1_2000.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, prod1_2000.RegisteredProducer, "B2C", 2, 1000);
                helper.CreateEeeOutputAmount(dataReturnVersion1, prod2_2000.RegisteredProducer, "B2C", 2, 400);

                //Current Year Data
                var memberUpload2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2001.ComplianceYear = 2001;
                var prod1_2001 = helper.CreateProducerAsCompany(memberUpload2001, "PRN123", "B2C");
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, 2001, 1);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, 2001, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion3, prod1_2001.RegisteredProducer, "B2C", 1, 3000);
                helper.CreateEeeOutputAmount(dataReturnVersion4, prod1_2001.RegisteredProducer, "B2C", 2, 600);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgSchemeObligationDataCsv(2001);

                //Assert
                Assert.NotNull(results);

                //Producer with only B2B (both years) should not be in data
                SchemeObligationCsvData unregisteredProd = results.Find(x => (x.PRN == "PRN456"));
                Assert.Null(unregisteredProd);
                Assert.Single(results);
            }
        }
    }
}

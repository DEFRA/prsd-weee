namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using RequestHandlers.Admin.Reports.GetMissingProducerDataCsv;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetMissingProducerDataCsvHandlerTests
    {
        [Fact]
        public async void DataProcessor_ReturnsMissingDataOnlyForSpecifiedCompianceYear()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload_2000 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload_2000.ComplianceYear = 2000;
                var prod_2000 = helper.CreateProducerAsCompany(memberUpload_2000, "PRN123", "B2B");

                var memberUpload_2001 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload_2001.ComplianceYear = 2001;
                var prod_2001 = helper.CreateProducerAsCompany(memberUpload_2001, "PRN123", "B2B");

                // Set up data for 2000 Q1/2/3 and 2001 Q4 - Expect to see missing 2000 Q4 result only
                var dataReturnVersion2000Q1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion2000Q1, prod_2000.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersion2000Q2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion2000Q2, prod_2000.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersion2000Q3 = helper.CreateDataReturnVersion(scheme, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersion2000Q3, prod_2000.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersion2001Q2 = helper.CreateDataReturnVersion(scheme, 2001, 4);
                helper.CreateEeeOutputAmount(dataReturnVersion2001Q2, prod_2001.RegisteredProducer, "B2B", 2, 600);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result = results[0];
                Assert.Equal(4, result.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsAllSchemes_WhenSchemeIdIsNull()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "Scheme1";
                var scheme2 = helper.CreateScheme();
                scheme2.SchemeName = "Scheme2";

                var memberUploadS1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUploadS1.ComplianceYear = 2000;
                var prodS1 = helper.CreateProducerAsCompany(memberUploadS1, "PRN123", "B2B");
                var memberUploadS2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUploadS2.ComplianceYear = 2000;
                var prodS2 = helper.CreateProducerAsCompany(memberUploadS2, "PRN456", "B2B");

                // Set up data for S1 Q1/2/3 and S2 Q1/2/3 - Expect to see missing Q4 results for both schemes
                var dataReturnVersionS1Q1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q1, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q2, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q3 = helper.CreateDataReturnVersion(scheme1, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q3, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS2Q1 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q1, prodS2.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS2Q2 = helper.CreateDataReturnVersion(scheme2, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q2, prodS2.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS2Q3 = helper.CreateDataReturnVersion(scheme2, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q3, prodS2.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal("Scheme1", result1.SchemeName);
                Assert.Equal(4, result1.Quarter);
                MissingProducerDataCsvData result2 = results[1];
                Assert.Equal("Scheme2", result2.SchemeName);
                Assert.Equal(4, result2.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsOnlySpecifiedScheme_WhenSchemeIdGiven()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "Scheme1";
                scheme1.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                var scheme2 = helper.CreateScheme();

                var memberUploadS1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUploadS1.ComplianceYear = 2000;
                var prodS1 = helper.CreateProducerAsCompany(memberUploadS1, "PRN123", "B2B");
                var memberUploadS2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUploadS2.ComplianceYear = 2000;
                var prodS2 = helper.CreateProducerAsCompany(memberUploadS2, "PRN456", "B2B");

                // Set up data for S1 Q1/2/3 and S2 Q1 - Expect to see missing Q4 result for scheme1 only
                var dataReturnVersionS1Q1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q1, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q2, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q3 = helper.CreateDataReturnVersion(scheme1, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q3, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS2Q1 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q1, prodS2.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, scheme1.Id);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result = results[0];
                Assert.Equal("Scheme1", result.SchemeName);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeRejectedSchemes()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "Scheme1";
                var scheme2 = helper.CreateScheme();
                scheme2.SchemeStatus = (int)SchemeStatus.Rejected;

                var memberUploadS1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUploadS1.ComplianceYear = 2000;
                var prodS1 = helper.CreateProducerAsCompany(memberUploadS1, "PRN123", "B2B");
                var memberUploadS2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUploadS2.ComplianceYear = 2000;
                var prodS2 = helper.CreateProducerAsCompany(memberUploadS2, "PRN456", "B2B");

                // Set up data for S1 Q1/2/3 and S2 Q1 - Expect to see missing Q4 result for scheme1 only
                var dataReturnVersionS1Q1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q1, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q2, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS1Q3 = helper.CreateDataReturnVersion(scheme1, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q3, prodS1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionS2Q1 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q1, prodS2.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal("Scheme1", result1.SchemeName);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsAllQuarters_WhenQuarterIsNull()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.SchemeName = "Scheme1";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod1 = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");
                var prod2 = helper.CreateProducerAsCompany(memberUpload, "PRN456", "B2B");

                // Set up data for P1 Q1/2/3 and P2 Q4 - Expect 4 results, one for each quarter
                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionQ2, prod1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ3 = helper.CreateDataReturnVersion(scheme, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionQ3, prod1.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ4 = helper.CreateDataReturnVersion(scheme, 2000, 4);
                helper.CreateEeeOutputAmount(dataReturnVersionQ4, prod2.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(4, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsOnlySpecifiedQuarter_WhenQuarterGiven()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 2, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result = results[0];
                Assert.Equal(2, result.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsAllQuartersForProducer_WhenNoDataReturnsUploaded()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(4, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsOnlyQuartersForProducer_ThatHaveNotHadDataReturnsUploaded()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0001ST/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                // Set up data for Q1 and Q4 - Expect to see Q2 and Q3 in results
                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ4 = helper.CreateDataReturnVersion(scheme, 2000, 4);
                helper.CreateEeeOutputAmount(dataReturnVersionQ4, prod.RegisteredProducer, "B2B", 2, 600);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal(2, result1.Quarter);
                MissingProducerDataCsvData result2 = results[1];
                Assert.Equal(3, result2.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsOnlyQuartersForProducer_AssociatedWithCorrectProducer()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0001ST/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod1 = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");
                prod1.Business.Company.Name = "Prod1";
                var prod2 = helper.CreateProducerAsCompany(memberUpload, "PRN456", "B2B");
                prod2.Business.Company.Name = "Prod2";

                // Prod1 given data for Q1,2,3. Prod 2 given data for Q2,3,4
                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod1.RegisteredProducer, "B2B", 1, 500); //Prod1, Q1
                var dataReturnVersionQ2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionQ2, prod1.RegisteredProducer, "B2B", 1, 500); //Prod1, Q2
                helper.CreateEeeOutputAmount(dataReturnVersionQ2, prod2.RegisteredProducer, "B2B", 1, 500); //Prod2, Q2
                var dataReturnVersionQ3 = helper.CreateDataReturnVersion(scheme, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionQ3, prod1.RegisteredProducer, "B2B", 1, 500); //Prod1, Q3
                helper.CreateEeeOutputAmount(dataReturnVersionQ3, prod2.RegisteredProducer, "B2B", 1, 500); //Prod2, Q3
                var dataReturnVersionQ4 = helper.CreateDataReturnVersion(scheme, 2000, 4);
                helper.CreateEeeOutputAmount(dataReturnVersionQ4, prod2.RegisteredProducer, "B2B", 1, 500); //Prod2, Q4

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal("Prod1", result1.ProducerName);
                Assert.Equal(4, result1.Quarter);
                MissingProducerDataCsvData result2 = results[1];
                Assert.Equal("Prod2", result2.ProducerName);
                Assert.Equal(1, result2.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeProducer_WhenAllDataReturnsUploaded()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                // Set up data for P1 Q1/2/3 and P2 Q4 - Expect 4 results, one for each quarter
                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionQ2, prod.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ3 = helper.CreateDataReturnVersion(scheme, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionQ3, prod.RegisteredProducer, "B2B", 1, 500);
                var dataReturnVersionQ4 = helper.CreateDataReturnVersion(scheme, 2000, 4);
                helper.CreateEeeOutputAmount(dataReturnVersionQ4, prod.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeQuarter_WithExplicitZeroDataSubmission()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2B", 1, 0);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 1, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsMissingB2B_WhenOnlyB2CProvidedForSchemeWithBothObligation()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "Both");

                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2C", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal(1, result1.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsMissingB2C_WhenOnlyB2BProvidedForSchemeWithBothObligation()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "Both");

                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2C", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal(1, result1.Quarter);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeProducerWithBothObligation_WhenB2CSpecifiedAndSubmitted_AndProducerIsMissingB2B()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "Both");

                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2C", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2C", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeProducerWithBothObligation_WhenB2BSpecifiedAndSubmitted_AndProducerIsMissingB2C()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod = helper.CreateProducerAsCompany(memberUpload, "PRN123", "Both");

                // Set up data for P1 Q1/2/3 and P2 Q4 - Expect 4 results, one for each quarter
                var dataReturnVersionQ1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionQ1, prod.RegisteredProducer, "B2B", 1, 500);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeB2BOnlyProducer_WhenB2CSpecified()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2C", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_DoesNotIncludeB2COnlyProducer_WhenB2BSpecified()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2C");

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                List<MissingProducerDataCsvData> results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 1, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async void DataProcessor_IncludesProducerThatHadSubmittedData_WhereSubmittedDataHasBeenRemovedInLatestSubmission()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0001ST/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;
                var prod1 = helper.CreateProducerAsCompany(memberUpload, "PRN123", "B2B");
                var prod2 = helper.CreateProducerAsCompany(memberUpload, "PRN456", "B2B");

                // Prod1 given data for Q1, then Q1 data overwritten with just Prod2 - Expect just Prod1 Q1 missing
                var dataReturnVersionFirst = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionFirst, prod1.RegisteredProducer, "B2B", 1, 500); //Prod1, Q1
                var dataReturnVersionSecond = helper.CreateDataReturnVersion(scheme, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionSecond, prod2.RegisteredProducer, "B2B", 1, 500); //Prod2, Q1

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", 1, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal("PRN123", result1.PRN);
            }
        }

        [Fact]
        public async void DataProcessor_ReturnsCorrectResultsInCorrectOrder()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "Scheme1";
                var scheme2 = helper.CreateScheme();
                scheme2.SchemeName = "Scheme2";

                var memberUploadS1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUploadS1.ComplianceYear = 2000;
                var prod2S1 = helper.CreateProducerAsCompany(memberUploadS1, "PRN123", "B2B");
                prod2S1.Business.Company.Name = "Prod2";
                var memberUploadS2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUploadS2.ComplianceYear = 2000;
                var prod3S2 = helper.CreateProducerAsCompany(memberUploadS2, "PRN456", "B2B");
                prod3S2.Business.Company.Name = "Prod3";
                var prod1S2 = helper.CreateProducerAsCompany(memberUploadS2, "PRN789", "B2B");
                prod1S2.Business.Company.Name = "Prod1";

                // Set up data for P1 Q1/2/3, P2 Q1/2, P3 Q1/2/3
                var dataReturnVersionS2Q1 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q1, prod3S2.RegisteredProducer, "B2B", 1, 500);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q1, prod1S2.RegisteredProducer, "B2B", 4, 40);
                var dataReturnVersionS2Q3 = helper.CreateDataReturnVersion(scheme2, 2000, 3);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q3, prod3S2.RegisteredProducer, "B2B", 3, 321);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q3, prod1S2.RegisteredProducer, "B2B", 6, 420);
                var dataReturnVersionS2Q2 = helper.CreateDataReturnVersion(scheme2, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q2, prod3S2.RegisteredProducer, "B2B", 9, 600);
                helper.CreateEeeOutputAmount(dataReturnVersionS2Q2, prod1S2.RegisteredProducer, "B2B", 12, 100);

                var dataReturnVersionS1Q1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q1, prod2S1.RegisteredProducer, "B2B", 4, 0);
                var dataReturnVersionS1Q2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                helper.CreateEeeOutputAmount(dataReturnVersionS1Q2, prod2S1.RegisteredProducer, "B2B", 9, 0);

                db.Model.SaveChanges();

                // Act
                GetMissingProducerDataCsvDataProcessor missingDataAccess = new GetMissingProducerDataCsvDataProcessor(db.WeeeContext);
                var results = await missingDataAccess.FetchMissingProducerDataAsync(2000, "B2B", null, null);

                // Assert
                Assert.NotNull(results);
                // ordered by scheme name, then producer name, then quarter
                Assert.Equal(4, results.Count);
                MissingProducerDataCsvData result1 = results[0];
                Assert.Equal("Scheme1", result1.SchemeName);
                Assert.Equal("Prod2", result1.ProducerName);
                Assert.Equal(3, result1.Quarter);
                MissingProducerDataCsvData result2 = results[1];
                Assert.Equal("Scheme1", result2.SchemeName);
                Assert.Equal("Prod2", result2.ProducerName);
                Assert.Equal(4, result2.Quarter);
                MissingProducerDataCsvData result3 = results[2];
                Assert.Equal("Scheme2", result3.SchemeName);
                Assert.Equal("Prod1", result3.ProducerName);
                Assert.Equal(4, result3.Quarter);
                MissingProducerDataCsvData result4 = results[3];
                Assert.Equal("Scheme2", result4.SchemeName);
                Assert.Equal("Prod3", result4.ProducerName);
                Assert.Equal(4, result4.Quarter);
            }
        }
    }
}

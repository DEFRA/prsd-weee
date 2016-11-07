namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Threading.Tasks;
    using RequestHandlers.Admin.GetDataReturnSubmissionChanges;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetDataReturnSubmissionEeeChangesCsvDataAccessTests
    {
        [Fact]
        public async Task GetChanges_WithInvalidPreviousSubmissionId_ThrowsInvalidOperationException()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act, Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetChanges(currentDataReturnVersion.Id, Guid.NewGuid()));
            }
        }

        [Fact]
        public async Task GetChanges_WithInvalidCurrentSubmissionId_ThrowsInvalidOperationException()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act, Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetChanges(Guid.NewGuid(), previousDataReturnVersion.Id));
            }
        }

        [Fact]
        public async Task GetChanges_WithDifferentSchemes_ThrowsInvalidOperationException()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme1, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme2, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act, Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id));
            }
        }

        [Fact]
        public async Task GetChanges_WithDifferentComplianceYears_ThrowsInvalidOperationException()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                MemberUpload memberUpload2 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2017;

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2017, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act, Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id));
            }
        }

        [Fact]
        public async Task GetChanges_WithDifferentQuarters_ThrowsInvalidOperationException()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 2, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act, Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id));
            }
        }

        [Fact]
        public async Task GetChanges_WithNoEeeForBothSubmissions_ReturnsEmptyResult()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Empty(results.CsvData);
            }
        }

        [Fact]
        public async Task GetChanges_WithNoEeeForPreviousSubmission_AndDataForCurrentSubmission_ReturnsDataAsInserted()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                var eeeOutputAmount1 = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 1, 10);
                var eeeOutputAmount2 = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 2, 20);

                helper.AddEeeOutputAmount(currentDataReturnVersion, eeeOutputAmount1);
                helper.AddEeeOutputAmount(currentDataReturnVersion, eeeOutputAmount2);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Single(results.CsvData);
                Assert.Equal(DataReturnSubmissionChangeType.Inserted, results.CsvData[0].ChangeType);
                Assert.Equal(10, results.CsvData[0].Cat1B2B);
                Assert.Equal(20, results.CsvData[0].Cat2B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_WithNoEeeForCurrentSubmission_AndDataForPreviousSubmission_ReturnsDataAsRemoved()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");

                var eeeOutputAmount1 = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 1, 10);
                var eeeOutputAmount2 = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 2, 20);

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount1);
                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount2);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Single(results.CsvData);
                Assert.Equal(DataReturnSubmissionChangeType.Removed, results.CsvData[0].ChangeType);
                Assert.Null(results.CsvData[0].Cat1B2B);
                Assert.Null(results.CsvData[0].Cat2B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_WithNoEeeChanges_ReturnsEmptyResult()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");

                var eeeOutputAmount = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 1, 10);

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);
                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);
                helper.AddEeeOutputAmount(currentDataReturnVersion, eeeOutputAmount);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Empty(results.CsvData);
            }
        }

        [Fact]
        public async Task GetChanges_WithAddedProducerEeeAmount_ReturnsRowWithAddedProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB22");

                var eeeOutputAmount = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 1, 10);

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);
                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);
                helper.AddEeeOutputAmount(currentDataReturnVersion, eeeOutputAmount);

                helper.CreateEeeOutputAmount(currentDataReturnVersion, producer2.RegisteredProducer, "B2B", 2, 20);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Single(results.CsvData);
                Assert.Equal(DataReturnSubmissionChangeType.Inserted, results.CsvData[0].ChangeType);
                Assert.Equal(20, results.CsvData[0].Cat2B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_WithEeeAmountChanged_ReturnsChangedAndPreviousRecord()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);
                helper.CreateEeeOutputAmount(previousDataReturnVersion, producer.RegisteredProducer, "B2B", 1, 10);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);
                helper.CreateEeeOutputAmount(currentDataReturnVersion, producer.RegisteredProducer, "B2B", 1, 12);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Equal(2, results.CsvData.Count);

                Assert.Equal(DataReturnSubmissionChangeType.Amended, results.CsvData[0].ChangeType);
                Assert.Equal(12, results.CsvData[0].Cat1B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);

                Assert.Null(results.CsvData[1].ChangeType);
                Assert.Equal(10, results.CsvData[1].Cat1B2B);
                Assert.Equal(new DateTime(2016, 1, 1), results.CsvData[1].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_WithRemovedProducerEeeAmount_ReturnsRowWithRemovedProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB11");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBBB22");

                var eeeOutputAmount1 = helper.CreateEeeOutputAmount(producer.RegisteredProducer, "B2B", 1, 10);
                var eeeOutputAmount2 = helper.CreateEeeOutputAmount(producer2.RegisteredProducer, "B2B", 2, 20);

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);
                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount1);
                helper.AddEeeOutputAmount(previousDataReturnVersion, eeeOutputAmount2);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);
                helper.AddEeeOutputAmount(currentDataReturnVersion, eeeOutputAmount1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Single(results.CsvData);
                Assert.Equal(DataReturnSubmissionChangeType.Removed, results.CsvData[0].ChangeType);
                Assert.Null(results.CsvData[0].Cat2B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_ReturnsRecordsSortedByProducerNameFollowedByLatestSubmissionDate()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload, "WEE/11AAA11");
                producer1.Business.Company.Name = "AAA";

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);
                helper.CreateEeeOutputAmount(previousDataReturnVersion, producer1.RegisteredProducer, "B2B", 1, 10);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);
                helper.CreateEeeOutputAmount(currentDataReturnVersion, producer1.RegisteredProducer, "B2B", 1, 12);

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "WEE/11BBB11");
                producer2.Business.Company.Name = "BBB";

                helper.CreateEeeOutputAmount(currentDataReturnVersion, producer2.RegisteredProducer, "B2B", 1, 30);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Equal(3, results.CsvData.Count);

                Assert.Equal("AAA", results.CsvData[0].ProducerName);
                Assert.Equal(DataReturnSubmissionChangeType.Amended, results.CsvData[0].ChangeType);
                Assert.Equal(12, results.CsvData[0].Cat1B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[0].SubmissionDate);

                Assert.Equal("AAA", results.CsvData[1].ProducerName);
                Assert.Null(results.CsvData[1].ChangeType);
                Assert.Equal(10, results.CsvData[1].Cat1B2B);
                Assert.Equal(new DateTime(2016, 1, 1), results.CsvData[1].SubmissionDate);

                Assert.Equal("BBB", results.CsvData[2].ProducerName);
                Assert.Equal(DataReturnSubmissionChangeType.Inserted, results.CsvData[2].ChangeType);
                Assert.Equal(30, results.CsvData[2].Cat1B2B);
                Assert.Equal(new DateTime(2016, 2, 1), results.CsvData[2].SubmissionDate);
            }
        }

        [Fact]
        public async Task GetChanges_ReturnsSubmissionDetails()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "ABC";

                MemberUpload memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var previousDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                previousDataReturnVersion.SubmittedDate = new DateTime(2016, 1, 1);

                var currentDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                currentDataReturnVersion.SubmittedDate = new DateTime(2016, 2, 1);

                db.Model.SaveChanges();

                var dataAccess = new GetDataReturnSubmissionEeeChangesCsvDataAccess(db.WeeeContext);

                // Act
                var results = await dataAccess.GetChanges(currentDataReturnVersion.Id, previousDataReturnVersion.Id);

                // Assert
                Assert.Equal("ABC", results.SchemeApprovalNumber);
                Assert.Equal(2016, results.ComplianceYear);
                Assert.Equal(1, results.Quarter);
                Assert.Equal(new DateTime(2016, 2, 1), results.CurrentSubmissionDate);
            }
        }
    }
}

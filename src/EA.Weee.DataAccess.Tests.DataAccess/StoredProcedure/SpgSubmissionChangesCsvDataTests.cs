namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgSubmissionChangesCsvDataTests
    {
        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsNew_WhenNoExistingProducerSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal("New", result[0].ChangeType);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsNew_WhenExistingProducerUploadButNoSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload1 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                modelHelper.CreateProducerAsCompany(memberUpload1, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal("New", result[0].ChangeType);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsNew_ProducerRegisteredForDifferentSchemeWithinSameYear_ButFirstSubmissionForCurrentScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111", "B2B");

                var scheme2 = modelHelper.CreateScheme();

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111", "B2C");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal("New", result[0].ChangeType);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsNew_WhenRegisteredForSameSchemeInAnotherYear_ButNoExistingProducerSubmissionForCurrentYear()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2017;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal("New", result[0].ChangeType);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_WhenSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload2, "2222");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Equal(3, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 1), result[1].SubmittedDate);

                Assert.Equal("New", result[2].ChangeType);
                Assert.Equal("2222", result[2].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[2].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsNew_WhenRemovedPriorToSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "1111");
                producer.RegisteredProducer.Removed = true;
                producer.RegisteredProducer.RemovedDate = new DateTime(2016, 1, 2);

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal("New", result[0].ChangeType);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_WhenRemovedButReaddedPriorToSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "1111");
                producer.RegisteredProducer.Removed = true;
                producer.RegisteredProducer.RemovedDate = new DateTime(2016, 1, 2);

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload3.Id);

                // Assert
                Assert.Equal(2, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal(new DateTime(2016, 3, 3), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal(new DateTime(2016, 2, 2), result[1].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_AndDetailsOfPreviousAndMostRecentSubmittedUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");

                // Member upload which will be ignored as it is submitted after
                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.SubmittedDate = new DateTime(2016, 4, 4);

                modelHelper.CreateProducerAsCompany(memberUpload4, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload3.Id);

                // Assert
                Assert.Equal(2, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 2, 2), result[1].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_AndDetailsOfPreviousAndMostRecentSubmittedUpload_ForEachProducer()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload, "2222");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 1, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload2, "2222");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload3, "2222");

                // Member upload which will be ignored as it is submitted after
                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload4, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload4, "2222");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload3.Id);

                // Assert
                Assert.Equal(4, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 2, 2), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 2), result[1].SubmittedDate);

                Assert.Equal("Amended", result[2].ChangeType);
                Assert.Equal("2222", result[2].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 2, 2), result[2].SubmittedDate);

                Assert.Equal(string.Empty, result[3].ChangeType);
                Assert.Equal("2222", result[3].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 2), result[3].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_AndDetailsOfPreviousAndMostRecentSubmittedUploadAffectingProducer_ForEachProducer()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload, "2222");

                // Member upload does not affect producer 2222
                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 1, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload3, "2222");

                // Member upload which will be ignored as it is submitted after
                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload4, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload4, "2222");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload3.Id);

                // Assert
                Assert.Equal(4, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 2, 2), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 2), result[1].SubmittedDate);

                Assert.Equal("Amended", result[2].ChangeType);
                Assert.Equal("2222", result[2].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 2, 2), result[2].SubmittedDate);

                Assert.Equal(string.Empty, result[3].ChangeType);
                Assert.Equal("2222", result[3].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 1), result[3].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsProducerAsChanged_AndDetailsOfPreviousSubmittedUpload_AndIgnoresUnsubmittedUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                // Un-submitted member upload, which should be ignored
                var memberUpload2 = modelHelper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = false;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");

                // Member upload which will be ignored as it is submitted after
                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.SubmittedDate = new DateTime(2016, 4, 4);

                modelHelper.CreateProducerAsCompany(memberUpload4, "1111");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload3.Id);

                // Assert
                Assert.Equal(2, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 1), result[1].SubmittedDate);
            }
        }

        [Fact]
        public async void SpgSubmissionChangesCsvData_ReturnsRecordsSortedByProducerRegistrationNumberAndSubmissionDateDescending()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload2, "2222");

                database.Model.SaveChanges();

                StoredProcedures storedProcedures = new StoredProcedures(database.WeeeContext);

                // Act
                var result = await storedProcedures.SpgSubmissionChangesCsvData(memberUpload2.Id);

                // Assert
                Assert.Equal(3, result.Count);

                Assert.Equal("Amended", result[0].ChangeType);
                Assert.Equal("1111", result[0].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[0].SubmittedDate);

                Assert.Equal(string.Empty, result[1].ChangeType);
                Assert.Equal("1111", result[1].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 1, 1), result[1].SubmittedDate);

                Assert.Equal("New", result[2].ChangeType);
                Assert.Equal("2222", result[2].ProducerRegistrationNumber);
                Assert.Equal(new DateTime(2016, 3, 3), result[2].SubmittedDate);
            }
        }
    }
}
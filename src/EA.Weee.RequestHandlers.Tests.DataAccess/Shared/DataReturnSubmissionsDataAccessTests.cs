namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using System;
    using System.Threading.Tasks;
    using RequestHandlers.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class DataReturnSubmissionsDataAccessTests
    {
        [Fact]
        public async Task GetPreviousSubmission_ReturnsNullIfNoPreviousSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetPreviousSubmission_ReturnsNullIfPreviousUploadButNoSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, false);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 2, 1);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion2.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetPreviousSubmission_ReturnsPreviousSubmissionForSameSchemeOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme1 = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                dataReturnVersion.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 3, 1);

                var scheme2 = modelHelper.CreateScheme();

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme2, 2015, 1, true);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 2, 1);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion2.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dataReturnVersion.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetPreviousSubmission_ReturnsPreviousSubmissionForSameYearOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2016, 1, true);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 3, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 5, 1);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion3.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dataReturnVersion.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetPreviousSubmission_ReturnsPreviousSubmissionForSameQuarterOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 3, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 5, 1);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion3.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dataReturnVersion.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetPreviousSubmission_ReturnsPreviousSubmissionWithClosestEarliestSubmissionDateTime()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                DataReturnSubmissionsDataAccess dataAccess = new DataReturnSubmissionsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 3, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 5, 1);

                var dataReturnVersion4 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                dataReturnVersion4.SubmittedDate = new DateTime(2015, 6, 1);

                database.Model.SaveChanges();

                var domainDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion3.Id);

                // Act
                var result = await dataAccess.GetPreviousSubmission(domainDataReturnVersion);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dataReturnVersion2.Id, result.Id);
            }
        }
    }
}

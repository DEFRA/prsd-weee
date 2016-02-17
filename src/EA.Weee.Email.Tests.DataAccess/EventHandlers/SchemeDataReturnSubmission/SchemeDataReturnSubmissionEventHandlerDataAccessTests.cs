namespace EA.Weee.Email.Tests.DataAccess.EventHandlers.SchemeDataReturnSubmission
{
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Email.EventHandlers.SchemeDataReturnSubmission;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SchemeDataReturnSubmissionEventHandlerDataAccessTests
    {
        [Fact]
        public async Task GetNumberOfDataReturnSubmissionsAsync_ReturnsDataForSubmittedDataReturnOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme, 2016, 1, false);
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);

                database.Model.SaveChanges();

                var dataAccess = new SchemeDataReturnSubmissionEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetNumberOfDataReturnSubmissionsAsync(domainHelper.GetScheme(scheme.Id), 2016, QuarterType.Q1);

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetNumberOfDataReturnSubmissionsAsync_ReturnsDataForSpecifiedSchemeOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme1 = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme1, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme1, 2016, 1, true);

                var scheme2 = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme2, 2016, 1, true);

                database.Model.SaveChanges();

                var dataAccess = new SchemeDataReturnSubmissionEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetNumberOfDataReturnSubmissionsAsync(domainHelper.GetScheme(scheme2.Id), 2016, QuarterType.Q1);

                // Assert
                Assert.Equal(1, result);
            }
        }

        [Fact]
        public async Task GetNumberOfDataReturnSubmissionsAsync_ReturnsDataForSpecifiedComplianceYearOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme, 2017, 1, true);

                database.Model.SaveChanges();

                var dataAccess = new SchemeDataReturnSubmissionEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetNumberOfDataReturnSubmissionsAsync(domainHelper.GetScheme(scheme.Id), 2017, QuarterType.Q1);

                // Assert
                Assert.Equal(1, result);
            }
        }

        [Fact]
        public async Task GetNumberOfDataReturnSubmissionsAsync_ReturnsDataForSpecifiedQuarterOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);
                helper.CreateDataReturnVersion(scheme, 2016, 3, true);

                database.Model.SaveChanges();

                var dataAccess = new SchemeDataReturnSubmissionEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetNumberOfDataReturnSubmissionsAsync(domainHelper.GetScheme(scheme.Id), 2016, QuarterType.Q3);

                // Assert
                Assert.Equal(1, result);
            }
        }
    }
}

namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System.Linq;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SchemeDataAccessTests
    {
        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeHasNoMemberUploads_ReturnsNoYears()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(scheme.Id);

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeHasUnsubmittedMemberUpload_ReturnsNoYears()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();
                var memberUpload = modelHelper.CreateMemberUpload(scheme);

                memberUpload.IsSubmitted = false;

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(scheme.Id);

                // Assert
                Assert.Empty(result);
            }   
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeHasSubmittedMemberUpload_ReturnsCorrespondingComplianceYear()
        {
            const int complianceYear = 1923;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();
                var memberUpload = modelHelper.CreateMemberUpload(scheme);

                memberUpload.IsSubmitted = true;
                memberUpload.ComplianceYear = complianceYear;

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(scheme.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal(complianceYear, result.Single());
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeWithTwoSubmittedMemberUploadsInSameYear_ReturnsYearOnce()
        {
            const int complianceYear = 1956;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();

                var firstMemberUpload = modelHelper.CreateMemberUpload(scheme);
                firstMemberUpload.IsSubmitted = true;
                firstMemberUpload.ComplianceYear = complianceYear;

                var secondMemberUpload = modelHelper.CreateMemberUpload(scheme);
                secondMemberUpload.IsSubmitted = true;
                secondMemberUpload.ComplianceYear = complianceYear;

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(scheme.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal(complianceYear, result.Single());
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeWithTwoSubmittedMemberUploadsInDifferentYears_ReturnsBothYears()
        {
            const int firstComplianceYear = 1921;
            const int secondComplianceYear = 1922;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();

                var firstMemberUpload = modelHelper.CreateMemberUpload(scheme);
                firstMemberUpload.IsSubmitted = true;
                firstMemberUpload.ComplianceYear = firstComplianceYear;

                var secondMemberUpload = modelHelper.CreateMemberUpload(scheme);
                secondMemberUpload.IsSubmitted = true;
                secondMemberUpload.ComplianceYear = secondComplianceYear;

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(scheme.Id);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(firstComplianceYear, result);
                Assert.Contains(secondComplianceYear, result);
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedDataReturns_SchemeHasNoDataReturns_ReturnsNoYears()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedDataReturns(scheme.Id);

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedDataReturns_SchemeHasSubmittedDataReturn_ReturnsCorrespondingComplianceYear()
        {
            const int complianceYear = 1666;
            const int quarter = 4;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();
                var dataReturn = modelHelper.CreateDataReturn(scheme, complianceYear, quarter);
                modelHelper.CreateDataReturnVersion(scheme, complianceYear, quarter, true,
                    dataReturn);

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedDataReturns(scheme.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal(complianceYear, result.Single());
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedDataReturns_SchemeWithTwoSubmittedDataReturnsUploadsInSameYear_ReturnsYearOnce()
        {
            const int complianceYear = 1888;
            const int firstDataReturnQuarter = 1;
            const int secondDataReturnQuarter = 2;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();
                var firstDataReturn = modelHelper.CreateDataReturn(scheme, complianceYear, firstDataReturnQuarter);
                var secondDataReturn = modelHelper.CreateDataReturn(scheme, complianceYear, secondDataReturnQuarter);

                modelHelper.CreateDataReturnVersion(scheme, complianceYear, firstDataReturnQuarter, true,
                    firstDataReturn);

                modelHelper.CreateDataReturnVersion(scheme, complianceYear, secondDataReturnQuarter, true,
                    secondDataReturn);

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedDataReturns(scheme.Id);

                // Assert
                Assert.Single(result);
                Assert.Equal(complianceYear, result.Single());
            }
        }

        [Fact]
        public async void GetComplianceYearsWithSubmittedMemberUploads_SchemeWithTwoSubmittedDataReturnsInDifferentYears_ReturnsBothYears()
        {
            const int firstComplianceYear = 1765;
            const int secondComplianceYear = 1766;
            const int quarter = 4;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var scheme = modelHelper.CreateScheme();
                var firstDataReturn = modelHelper.CreateDataReturn(scheme, firstComplianceYear, quarter);
                var secondDataReturn = modelHelper.CreateDataReturn(scheme, secondComplianceYear, quarter);

                modelHelper.CreateDataReturnVersion(scheme, firstComplianceYear, quarter, true,
                    firstDataReturn);

                modelHelper.CreateDataReturnVersion(scheme, secondComplianceYear, quarter, true,
                    secondDataReturn);

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new SchemeDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetComplianceYearsWithSubmittedDataReturns(scheme.Id);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(firstComplianceYear, result);
                Assert.Contains(secondComplianceYear, result);
            }
        }
    }
}

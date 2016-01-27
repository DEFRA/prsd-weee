namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.FetchDataReturnComplianceYearsForScheme
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using EA.Weee.Tests.Core.Model;
    using RequestHandlers.DataReturns.FetchDataReturnComplianceYearsForScheme;
    using Xunit;
    public class FetchDataReturnComplianceYearsForSchemeDataAccessTests
    {
        [Fact]
        public async Task FetchSchemeByOrganisationIdAsync_GetsSchemeForMatchingOrganisationId()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var organisation1 = helper.CreateOrganisation();
                helper.CreateScheme(organisation1);

                var organisation2 = helper.CreateOrganisation();
                var scheme2 = helper.CreateScheme(organisation2);

                database.Model.SaveChanges();

                var dataAccess = new FetchDataReturnComplianceYearsForSchemeDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchSchemeByOrganisationIdAsync(organisation2.Id);

                // Assert
                Assert.Equal(scheme2.Id, result.Id);
            }
        }

        [Fact]
        public async void Fetch_TwoUploadsForTheSameComplianceYear_OnlyReturnsTheYearOnce()
        {
            using (var database = new DatabaseWrapper())
            {
                const int complianceYear = 2005;

                // Arrange
                var modelHelper = new ModelHelper(database.Model);

                var organisation = modelHelper.CreateOrganisation();
                await database.Model.SaveChangesAsync();

                var scheme = modelHelper.CreateScheme(organisation);
                await database.Model.SaveChangesAsync();

                var firstDataReturn = modelHelper.CreateDataReturn(scheme, complianceYear, (int)QuarterType.Q1);
                await database.Model.SaveChangesAsync();

                var firstDataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, complianceYear, (int)QuarterType.Q1, true, firstDataReturn);
                modelHelper.CreateDataReturnUpload(scheme, firstDataReturnVersion);
                await database.Model.SaveChangesAsync();

                var secondDataReturn = modelHelper.CreateDataReturn(scheme, complianceYear, (int)QuarterType.Q2);
                await database.Model.SaveChangesAsync();

                var secondDataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, complianceYear, (int)QuarterType.Q2, true, secondDataReturn);
                modelHelper.CreateDataReturnUpload(scheme, secondDataReturnVersion);
                await database.Model.SaveChangesAsync();

                // Act
                var dataAccess = new FetchDataReturnComplianceYearsForSchemeDataAccess(database.WeeeContext);
                var result = await dataAccess.GetDataReturnComplianceYearsForScheme(scheme.Id);

                // Assert
                Assert.Single(result.Where(y => y == complianceYear));
            }
        }

        [Fact]
        public async Task GetDataReturnComplianceYearsForSchemeHandler_ReturnsYearsInDescendingOrder()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);                

                var scheme = helper.CreateScheme();
                var firstDataReturn = helper.CreateDataReturn(scheme, 2006, (int)QuarterType.Q1);
                var firstDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2006, 1, true, firstDataReturn);
                helper.CreateDataReturnUpload(scheme, firstDataReturnVersion);

                var secondDataReturn = helper.CreateDataReturn(scheme, 2007, (int)QuarterType.Q1);
                var secondDataReturnVersion = helper.CreateDataReturnVersion(scheme, 2007, 1, true, secondDataReturn);
                helper.CreateDataReturnUpload(scheme, secondDataReturnVersion);
                database.Model.SaveChanges();               

                // Act
                var dataAccess = new FetchDataReturnComplianceYearsForSchemeDataAccess(database.WeeeContext);
                var yearsList = await dataAccess.GetDataReturnComplianceYearsForScheme(scheme.Id);

                //Assert                
                Assert.NotNull(yearsList);
                Assert.Equal(2, yearsList.Count);
                Assert.Collection(yearsList,
                    r1 => Assert.Equal("2007", r1.ToString()),
                    r2 => Assert.Equal("2006", r2.ToString()));
            }
        }
    }
}

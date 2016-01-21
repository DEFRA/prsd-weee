namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetActiveComplianceYears
{
    using System;
    using System.Linq;
    using Core.DataReturns;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetDataReturnsActiveComplianceYearsDataAccessTests
    {
        [Fact]
        public async void Get_TwoUploadsForTheSameComplianceYear_OnlyReturnsTheYearOnce()
        {
            using (var database = new DatabaseWrapper())
            {
                const int complianceYear = 2995;

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
                var dataAccess = new GetDataReturnsActiveComplianceYearsDataAccess(database.WeeeContext);
                var result = await dataAccess.Get();

                // Assert
                Assert.Single(result.Where(y => y == complianceYear));
            }
        }
    }
}

﻿namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetActiveComplianceYears
{
    using Xunit;
    using System.Linq;
    using Core.DataReturns;
    using Weee.Tests.Core.Model;
    using System.Threading.Tasks;
    using RequestHandlers.Admin.GetActiveComplianceYears;

    public class GetAatfReturnsActiveComplianceYearsDataAccessTests
    {
        [Fact]
        public async Task Get_TwoUploadsForTheSameComplianceYear_OnlyReturnsTheYearOnce()
        {
            using (var database = new DatabaseWrapper())
            {
                const int complianceYear = 2995;

                // Arrange
                var modelHelper = new ModelHelper(database.Model);

                var organisation = modelHelper.CreateOrganisation();
                await database.Model.SaveChangesAsync();

                var user = modelHelper.CreateUser("two-uploads");
                await database.Model.SaveChangesAsync();

                var firstDataReturn = modelHelper.CreateAatfReturn(organisation, complianceYear, (int)QuarterType.Q1, user.Id);
                await database.Model.SaveChangesAsync();

                var secondDataReturn = modelHelper.CreateAatfReturn(organisation, complianceYear, (int)QuarterType.Q2, user.Id);
                await database.Model.SaveChangesAsync();

                // Act
                var dataAccess = new GetAatfReturnsActiveComplianceYearsDataAccess(database.WeeeContext);
                var result = await dataAccess.Get();

                // Assert
                Assert.Single(result.Where(y => y == complianceYear));
            }
        }
    }
}

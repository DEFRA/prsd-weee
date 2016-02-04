namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetActiveComplianceYears
{
    using System;
    using System.Linq;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetMemberRegistrationActiveComplianceYearsDataAccessTests
    {
        [Fact]
        public async void Get_TwoUploadsForTheSameComplianceYear_OnlyReturnsTheYearOnce()
        {
            using (var database = new DatabaseWrapper())
            {
                const int complianceYear = 2997;

                // Arrange
                var modelHelper = new ModelHelper(database.Model);

                var organisation = modelHelper.CreateOrganisation();
                await database.Model.SaveChangesAsync();

                var scheme = modelHelper.CreateScheme(organisation);
                await database.Model.SaveChangesAsync();

                var firstMemberUpload = modelHelper.CreateMemberUpload(scheme);

                firstMemberUpload.ComplianceYear = complianceYear;
                firstMemberUpload.IsSubmitted = true;
                firstMemberUpload.OrganisationId = organisation.Id;
                firstMemberUpload.CreatedDate = DateTime.Now;
                firstMemberUpload.CreatedById = database.WeeeContext.GetCurrentUser();

                var secondMemberUpload = modelHelper.CreateMemberUpload(scheme);

                secondMemberUpload.ComplianceYear = complianceYear;
                secondMemberUpload.IsSubmitted = true;
                secondMemberUpload.OrganisationId = organisation.Id;
                secondMemberUpload.CreatedDate = DateTime.Now;
                secondMemberUpload.CreatedById = database.WeeeContext.GetCurrentUser();

                await database.Model.SaveChangesAsync();

                // Act
                var dataAccess = new GetMemberRegistrationsActiveComplianceYearsDataAccess(database.WeeeContext);
                var result = await dataAccess.Get();

                // Assert
                Assert.Single(result.Where(y => y == complianceYear));
            }
        }
    }
}

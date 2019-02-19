namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using FluentAssertions;
    using RequestHandlers.Scheme;
    using Xunit;
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;
    using ModelHelper = Weee.Tests.Core.Model.ModelHelper;

    public class GetSchemesDataAccessTests
    {
        [Fact]
        public async Task GetSchemesDataAccess_GetCompleteSchemes_ReturnsOnlySchemesWithCompleteOrganisations()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var organisation1 = helper.CreateOrganisation();
                organisation1.OrganisationType = 3;
                organisation1.OrganisationStatus = 2; // Complete organisation
                var scheme1 = helper.CreateScheme(organisation1);

                var organisation2 = helper.CreateOrganisation();
                organisation2.OrganisationType = 1;
                organisation2.OrganisationStatus = 2; // Complete organisation
                var scheme2 = helper.CreateScheme(organisation2);

                var organisation3 = helper.CreateOrganisation();
                organisation3.OrganisationType = 1;
                organisation3.OrganisationStatus = 1; // Incomplete organisation
                var scheme3 = helper.CreateScheme(organisation3);

                var organisation4 = helper.CreateOrganisation();
                organisation4.OrganisationType = 2;
                organisation4.OrganisationStatus = 1; // Incomplete organisation
                var scheme4 = helper.CreateScheme(organisation4);

                database.Model.SaveChanges();

                var dataAccess = new GetSchemesDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetCompleteSchemes();

                // Assert
                Assert.Contains(result, r => r.Id == scheme1.Id);
                Assert.Contains(result, r => r.Id == scheme2.Id);
                Assert.DoesNotContain(result, r => r.Id == scheme3.Id);
                Assert.DoesNotContain(result, r => r.Id == scheme4.Id);
            }
        }
    }
}
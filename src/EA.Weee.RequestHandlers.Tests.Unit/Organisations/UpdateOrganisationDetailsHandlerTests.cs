namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Security;
    using DataAccess.Identity;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;
    using OrganisationType = Core.Organisations.OrganisationType;

    public class UpdateOrganisationDetailsHandlerTests
    {
        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            var handler = new UpdateOrganisationDetailsHandler(dataAccess, authorization);
            
            // Act
            Func<Task> action = async () => await handler.HandleAsync(A<UpdateOrganisationDetails>._);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();
            
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            var handler = new UpdateOrganisationDetailsHandler(dataAccess, authorization);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A<UpdateOrganisationDetails>._);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task UpdateOrganisationDetailsHandler_WithValidData_FetchesOrganisationAndUpdatesAndSaves()
        {
            // Arrange
            OrganisationData organisationData = new OrganisationData();
            organisationData.Id = new Guid("9a310218-311b-460d-bd50-9d246c237dcc");
            organisationData.OrganisationType = OrganisationType.RegisteredCompany;
            organisationData.Name = "CompanyName";
            organisationData.CompanyRegistrationNumber = "123456789";
            organisationData.OrganisationName = "CompanyName";
            organisationData.BusinessAddress = new Core.Shared.AddressData();
            organisationData.BusinessAddress.Address1 = "Address1";
            organisationData.BusinessAddress.Address2 = "Address2";
            organisationData.BusinessAddress.TownOrCity = "Town";
            organisationData.BusinessAddress.CountyOrRegion = "County";
            organisationData.BusinessAddress.Postcode = "Postcode";
            organisationData.BusinessAddress.CountryId = new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4");
            organisationData.BusinessAddress.Telephone = "012345678";
            organisationData.BusinessAddress.Email = "email@domain.com";

            UpdateOrganisationDetails request = new UpdateOrganisationDetails(organisationData);

            IOrganisationDetailsDataAccess dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            IWeeeAuthorization weeeAuthorization = A.Fake<IWeeeAuthorization>();

            Organisation organisation = A.Dummy<Organisation>();
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("9a310218-311b-460d-bd50-9d246c237dcc")))
                .Returns(organisation);

            Country country = new Country(
                new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4"),
                "Name");
            A.CallTo(() => dataAccess.FetchCountryAsync(new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4")))
                .Returns(country);

            UpdateOrganisationDetailsHandler handler = new UpdateOrganisationDetailsHandler(dataAccess, weeeAuthorization);

            // Act
            bool result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("9a310218-311b-460d-bd50-9d246c237dcc")))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal("CompanyName", organisation.Name);
            Assert.Equal("123456789", organisation.CompanyRegistrationNumber);
            Assert.Equal("Address1", organisation.BusinessAddress.Address1);
            Assert.Equal("Address2", organisation.BusinessAddress.Address2);
            Assert.Equal("Town", organisation.BusinessAddress.TownOrCity);
            Assert.Equal("County", organisation.BusinessAddress.CountyOrRegion);
            Assert.Equal("Postcode", organisation.BusinessAddress.Postcode);
            Assert.Equal(new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4"), organisation.BusinessAddress.Country.Id);
            Assert.Equal("012345678", organisation.BusinessAddress.Telephone);
            Assert.Equal("email@domain.com", organisation.BusinessAddress.Email);

            A.CallTo(() => dataAccess.SaveAsync())
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(result, true);
        }
    }
}

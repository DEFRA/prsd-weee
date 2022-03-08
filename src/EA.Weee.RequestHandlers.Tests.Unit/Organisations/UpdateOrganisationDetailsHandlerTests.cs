﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using FakeItEasy;
    using RequestHandlers.Security;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
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
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<UpdateOrganisationDetails>());

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
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<UpdateOrganisationDetails>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task UpdateOrganisationDetailsHandler_WithValidData_FetchesOrganisationAndUpdatesAndSaves()
        {
            // Arrange
            var organisationData = new OrganisationData
            {
                Id = new Guid("9a310218-311b-460d-bd50-9d246c237dcc"),
                OrganisationType = OrganisationType.RegisteredCompany,
                Name = "CompanyName",
                CompanyRegistrationNumber = "123456789",
                OrganisationName = "CompanyName",
                BusinessAddress = new Core.Shared.AddressData
                {
                    Address1 = "Address1",
                    Address2 = "Address2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "Postcode",
                    CountryId = new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4"),
                    Telephone = "012345678",
                    Email = "email@domain.com"
                }
            };

            var request = new UpdateOrganisationDetails(organisationData);

            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();

            var organisation = A.Dummy<Organisation>();
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("9a310218-311b-460d-bd50-9d246c237dcc")))
                .Returns(organisation);

            var country = new Country(
                new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4"),
                "Name");
            A.CallTo(() => dataAccess.FetchCountryAsync(new Guid("79b70dfb-bbfd-4801-9849-880f66ee48e4")))
                .Returns(country);

            var handler = new UpdateOrganisationDetailsHandler(dataAccess, weeeAuthorization);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("9a310218-311b-460d-bd50-9d246c237dcc")))
                .MustHaveHappened(1, Times.Exactly);

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
                .MustHaveHappened(1, Times.Exactly);

            Assert.Equal(result, true);
        }
    }
}

namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using Email;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Organisations;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateSchemeContactDetailsHandlerTests
    {
        [Fact]
        public void HandleAsync_GivenSchemeWithOrganisationIdNotFound_ArgumentExceptionExpected_AndEmailMustNotBeSent()
        {
            // Arrange
            var schemeData = new SchemeData
            {
                OrganisationId = Guid.NewGuid()
            };

            var request = new UpdateSchemeContactDetails(schemeData);
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var scheme = A.Dummy<Scheme>();

            A.CallTo(() => dataAccess.FetchSchemeAsync(schemeData.OrganisationId)).Returns((Scheme)null);

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalUserOrOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            var authorizationBuilder = new AuthorizationBuilder()
                .DenyInternalOrOrganisationAccess();

            var authorization = authorizationBuilder.Build();

            var dataAccess = A.Dummy<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler =
                new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var request = new UpdateSchemeContactDetails(new SchemeData() { Id = Guid.NewGuid() }, false);

            // Act, Assert
            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            // Arrange
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<UpdateSchemeContactDetails>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        /// <summary>
        /// This test ensures that the handler correctly updates each of the properties for the contact and organisation
        /// address of the specified organisation, before saving the changes.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithValidData_FetchesSchemeAndUpdatesAndSaves()
        {
            // Arrange
            var schemeData = new SchemeData
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                Contact = new ContactData { FirstName = "FirstName", LastName = "LastName", Position = "Position" },
                Address = new Core.Shared.AddressData
                {
                    Address1 = "Address1",
                    Address2 = "Address2",
                    TownOrCity = "Town",
                    CountyOrRegion = "County",
                    Postcode = "Postcode",
                    CountryId = Guid.NewGuid(),
                    Telephone = "012345678",
                    Email = "email@domain.com"
                }
            };

            var request = new UpdateSchemeContactDetails(schemeData);
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var scheme = A.Dummy<Scheme>();

            A.CallTo(() => dataAccess.FetchSchemeAsync(schemeData.OrganisationId)).Returns(scheme);

            var country = new Country(schemeData.Address.CountryId, "Name");
            A.CallTo(() => dataAccess.FetchCountryAsync(schemeData.Address.CountryId)).Returns(country);

            var handler =
                new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchSchemeAsync(schemeData.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal("FirstName", scheme.Contact.FirstName);
            Assert.Equal("LastName", scheme.Contact.LastName);
            Assert.Equal("Position", scheme.Contact.Position); 
            Assert.Equal("Address1", scheme.Address.Address1);
            Assert.Equal("Address2", scheme.Address.Address2);
            Assert.Equal("Town", scheme.Address.TownOrCity);
            Assert.Equal("County", scheme.Address.CountyOrRegion);
            Assert.Equal("Postcode", scheme.Address.Postcode);
            Assert.Equal(schemeData.Address.CountryId, scheme.Address.Country.Id);
            Assert.Equal("012345678", scheme.Address.Telephone);
            Assert.Equal("email@domain.com", scheme.Address.Email);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_WithContactDetailsChange_SendNotificationTrue_SendsChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);

            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "New FirstName",
                LastName = "New LastName",
                Position = "New Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "Address1",
                Address2 = "Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var organisationData = new SchemeData()
            {
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(organisationData, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithContactDetailsChange_SendNotificationFalse_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var contact = new Contact("FirstName", "LastName", "Position");
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "New FirstName",
                LastName = "New LastName",
                Position = "New Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "Address1",
                Address2 = "Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var schemeData = new SchemeData()
            {
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, false);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithSchemeAddressChange_SendNotificationTrue_SendsChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newSchemeAdddress = new AddressData
            {
                Address1 = "New Address1",
                Address2 = "New Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var schemeData = new SchemeData()
            {
                Contact = newContactDetails,
                Address = newSchemeAdddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithSchemeAddressChange_SendNotificationFalse_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);

            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "New Address1",
                Address2 = "New Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var schemeData = new SchemeData()
            {
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, false);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithSchemeAddressAndContactDetailsUnchanged_SendNotificationTrue_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);

            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "Address1",
                Address2 = "Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var schemeData = new SchemeData()
            {
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_DetailsChanged_SendNotificationTrue_MatchingSchemeNoCompetentAuthority_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "New Address1",
                Address2 = "New Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var organisationId = Guid.NewGuid();

            var schemeData = new SchemeData()
            {
                OrganisationId = organisationId,
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, true);

            A.CallTo(() => scheme.SchemeName).Returns("Test Scheme Name");
            A.CallTo(() => scheme.CompetentAuthority).Returns(null);
            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId)).Returns(scheme);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId)).MustHaveHappened();
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged("test@authorityEmail.gov.uk", "Test Scheme Name"))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_DetailsChanged_SendNotificationTrue_MatchingScheme_SendsChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateSchemeContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var schemeAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Contact).Returns(contact);
            A.CallTo(() => scheme.Address).Returns(schemeAddress);
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);
            A.CallTo(() => dataAccess.FetchCountryAsync(countryId)).Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newSchemeAddress = new AddressData
            {
                Address1 = "New Address1",
                Address2 = "New Address2",
                CountryId = countryId,
                CountyOrRegion = "CountyOrRegion",
                Email = "Email",
                Postcode = "Postcode",
                Telephone = "Telephone",
                TownOrCity = "TownOrCity"
            };

            var schemeId = Guid.NewGuid();

            var schemeData = new SchemeData()
            {
                Id = schemeId,
                Contact = newContactDetails,
                Address = newSchemeAddress
            };

            var request = new UpdateSchemeContactDetails(schemeData, true);

            A.CallTo(() => scheme.SchemeName).Returns("Test Scheme Name");
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            A.CallTo(() => competentAuthority.Email).Returns("test@authorityEmail.gov.uk");
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);
            A.CallTo(() => dataAccess.FetchSchemeAsync(schemeId)).Returns(scheme);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged("test@authorityEmail.gov.uk", "Test Scheme Name"))
                .MustHaveHappened();
        }
    }
}
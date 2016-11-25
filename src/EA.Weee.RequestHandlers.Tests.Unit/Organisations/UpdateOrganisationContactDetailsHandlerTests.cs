namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using Email;
    using FakeItEasy;
    using RequestHandlers.Security;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateOrganisationContactDetailsHandlerTests
    {
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
                new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var request = new UpdateOrganisationContactDetails(new OrganisationData { Id = Guid.NewGuid() }, false);

            // Act, Assert
            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(request));
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
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<UpdateOrganisationContactDetails>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        /// <summary>
        /// This test ensures that the handler correctly updates each of the properties for the contact and organisation
        /// address of the specified organisation, before saving the changes.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithValidData_FetchesOrganisationAndUpdatesAndSaves()
        {
            // Arrange
            OrganisationData organisationData = new OrganisationData();
            organisationData.Id = new Guid("93646500-85A1-4F9D-AE18-73265426EF40");
            organisationData.Contact = new ContactData();
            organisationData.Contact.FirstName = "FirstName";
            organisationData.Contact.LastName = "LastName";
            organisationData.Contact.Position = "Position";
            organisationData.OrganisationAddress = new Core.Shared.AddressData();
            organisationData.OrganisationAddress.Address1 = "Address1";
            organisationData.OrganisationAddress.Address2 = "Address2";
            organisationData.OrganisationAddress.TownOrCity = "Town";
            organisationData.OrganisationAddress.CountyOrRegion = "County";
            organisationData.OrganisationAddress.Postcode = "Postcode";
            organisationData.OrganisationAddress.CountryId = new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC");
            organisationData.OrganisationAddress.Telephone = "012345678";
            organisationData.OrganisationAddress.Email = "email@domain.com";

            UpdateOrganisationContactDetails request = new UpdateOrganisationContactDetails(organisationData);

            IWeeeAuthorization authorization = A.Dummy<IWeeeAuthorization>();
            IOrganisationDetailsDataAccess dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            IWeeeEmailService weeeEmailService = A.Dummy<IWeeeEmailService>();

            Organisation organisation = A.Dummy<Organisation>();

            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("93646500-85A1-4F9D-AE18-73265426EF40")))
                .Returns(organisation);

            Country country = new Country(
                new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC"),
                "Name");
            A.CallTo(() => dataAccess.FetchCountryAsync(new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC")))
                .Returns(country);

            UpdateOrganisationContactDetailsHandler handler =
                new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            // Act
            bool result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("93646500-85A1-4F9D-AE18-73265426EF40")))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal("FirstName", organisation.Contact.FirstName);
            Assert.Equal("LastName", organisation.Contact.LastName);
            Assert.Equal("Position", organisation.Contact.Position);
            Assert.Equal("Address1", organisation.OrganisationAddress.Address1);
            Assert.Equal("Address2", organisation.OrganisationAddress.Address2);
            Assert.Equal("Town", organisation.OrganisationAddress.TownOrCity);
            Assert.Equal("County", organisation.OrganisationAddress.CountyOrRegion);
            Assert.Equal("Postcode", organisation.OrganisationAddress.Postcode);
            Assert.Equal(new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC"), organisation.OrganisationAddress.Country.Id);
            Assert.Equal("012345678", organisation.OrganisationAddress.Telephone);
            Assert.Equal("email@domain.com", organisation.OrganisationAddress.Email);

            A.CallTo(() => dataAccess.SaveAsync())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_WithContactDetailsChange_SendNotificationTrue_SendsChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "New FirstName",
                LastName = "New LastName",
                Position = "New Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

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

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var contact = new Contact("FirstName", "LastName", "Position");
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "New FirstName",
                LastName = "New LastName",
                Position = "New Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, false);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithOrganisationAddressChange_SendNotificationTrue_SendsChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithOrganisationAddressChange_SendNotificationFalse_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, false);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithOrganisationAddressAndContactDetailsUnchanged_SendNotificationTrue_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_DetailsChanged_SendNotificationTrue_NoMatchingScheme_DoesNotSendChangeEmail()
        {
            // Arrange
            var authorization = A.Dummy<IWeeeAuthorization>();
            var dataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            var weeeEmailService = A.Dummy<IWeeeEmailService>();

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Id = organisationId,
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId))
                .Returns((Scheme)null);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId))
                .MustHaveHappened();

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

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Id = organisationId,
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.SchemeName)
                .Returns("Test Scheme Name");

            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(null);

            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId))
                .Returns(scheme);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId))
                .MustHaveHappened();

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

            var handler = new UpdateOrganisationContactDetailsHandler(authorization, dataAccess, weeeEmailService);

            var contact = new Contact("FirstName", "LastName", "Position");

            var countryId = Guid.NewGuid();
            var country = new Country(countryId, "Country");
            var organisationAddress = new Address("Address1", "Address2", "TownOrCity",
                "CountyOrRegion", "Postcode", country, "Telephone", "Email");

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Contact)
                .Returns(contact);

            A.CallTo(() => organisation.OrganisationAddress)
                .Returns(organisationAddress);

            A.CallTo(() => dataAccess.FetchOrganisationAsync(A<Guid>._))
                .Returns(organisation);

            A.CallTo(() => dataAccess.FetchCountryAsync(countryId))
                .Returns(country);

            var newContactDetails = new ContactData
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Position = "Position"
            };

            var newOrganisationAddress = new AddressData
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

            var organisationData = new OrganisationData
            {
                Id = organisationId,
                Contact = newContactDetails,
                OrganisationAddress = newOrganisationAddress
            };

            var request = new UpdateOrganisationContactDetails(organisationData, true);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.SchemeName)
                .Returns("Test Scheme Name");

            var competentAuthority = A.Fake<UKCompetentAuthority>();
            A.CallTo(() => competentAuthority.Email)
                .Returns("test@authorityEmail.gov.uk");

            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            A.CallTo(() => dataAccess.FetchSchemeAsync(organisationId))
                .Returns(scheme);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeEmailService.SendOrganisationContactDetailsChanged("test@authorityEmail.gov.uk", "Test Scheme Name"))
                .MustHaveHappened();
        }
    }
}
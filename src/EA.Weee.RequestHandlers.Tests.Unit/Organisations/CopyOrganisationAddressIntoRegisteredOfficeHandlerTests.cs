﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class CopyOrganisationAddressIntoRegisteredOfficeHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(denyingAuthorization, A.Dummy<WeeeContext>());
            var message = new CopyOrganisationAddressIntoRegisteredOffice(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_NoSuchAddress_ThrowsArgumentException()
        {
            var addressId = Guid.NewGuid();

            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Addresses).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Address>()));
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(permissiveAuthorization, context);
            var message = new CopyOrganisationAddressIntoRegisteredOffice(Guid.NewGuid(), addressId);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.Contains(addressId.ToString(), exception.Message);
            Assert.Contains("COULD NOT FIND", exception.Message.ToUpperInvariant());
            Assert.Contains("ADDRESS", exception.Message.ToUpperInvariant());
        }

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var organisationId = Guid.NewGuid();
            var address = MakeAddress();

            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));
            A.CallTo(() => context.Addresses).Returns(dbHelper.GetAsyncEnabledDbSet(address));

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(permissiveAuthorization, context);
            var message = new CopyOrganisationAddressIntoRegisteredOffice(organisationId, address.First().Id);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.Contains(organisationId.ToString(), exception.Message);
            Assert.Contains("COULD NOT FIND", exception.Message.ToUpperInvariant());
            Assert.Contains("ORGANISATION", exception.Message.ToUpperInvariant());
        }

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_CopiedOrganisationAddressToRegisteredOffice()
        {
            var organisations = MakeOrganisation();
            var addresses = MakeAddress();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);
            A.CallTo(() => context.Addresses).Returns(addresses);

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(permissiveAuthorization, context);

            var organisationId =
                await
                    handler.HandleAsync(
                        new CopyOrganisationAddressIntoRegisteredOffice(organisations.FirstOrDefault().Id, addresses.FirstOrDefault().Id));
            var organisationInfo = organisations.FirstOrDefault();
            var addressInfo = addresses.FirstOrDefault();

            Assert.NotNull(organisationInfo);
            
            Assert.Equal(organisationInfo.BusinessAddress.Address1, addressInfo.Address1);
            Assert.Equal(organisationInfo.BusinessAddress.Address2, addressInfo.Address2);
            Assert.Equal(organisationInfo.BusinessAddress.TownOrCity, addressInfo.TownOrCity);
            Assert.Equal(organisationInfo.BusinessAddress.CountyOrRegion, addressInfo.CountyOrRegion);
            Assert.Equal(organisationInfo.BusinessAddress.Postcode, addressInfo.Postcode);
            Assert.Equal(organisationInfo.BusinessAddress.Country, addressInfo.Country);
            Assert.Equal(organisationInfo.BusinessAddress.Telephone, addressInfo.Telephone);
            Assert.Equal(organisationInfo.BusinessAddress.Email, addressInfo.Email);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return dbHelper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("TEST Ltd")
            });
        }

        private DbSet<Address> MakeAddress()
        {
            var address = new Address("address1", "address2", "town", "county", "postcode", new Country(Guid.NewGuid(), "country"), "telephone", "email");

            return dbHelper.GetAsyncEnabledDbSet(new[]
            {
                address
            });
        }
    }
}

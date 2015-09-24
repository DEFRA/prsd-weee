namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;

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
            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(denyingAuthorization, A<WeeeContext>._);
            var message = new CopyOrganisationAddressIntoRegisteredOffice(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var organisationId = Guid.NewGuid();

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(permissiveAuthorization, context);
            var message = new CopyOrganisationAddressIntoRegisteredOffice(organisationId);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.Contains(organisationId.ToString()));
            Assert.True(exception.Message.ToUpperInvariant().Contains("COULD NOT FIND"));
            Assert.True(exception.Message.ToUpperInvariant().Contains("ORGANISATION"));
        }

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_CopiedOrganisationAddressToRegisteredOffice
            ()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(permissiveAuthorization, context);

            var organisationId =
                await
                    handler.HandleAsync(
                        new CopyOrganisationAddressIntoRegisteredOffice(organisations.FirstOrDefault().Id));
            var organisationInfo = organisations.FirstOrDefault();

            Assert.NotNull(organisationInfo);
            Assert.NotNull(organisationId);
            Assert.Equal(organisationInfo.BusinessAddress.Address1, organisationInfo.OrganisationAddress.Address1);
            Assert.Equal(organisationInfo.BusinessAddress.Address2, organisationInfo.OrganisationAddress.Address2);
            Assert.Equal(organisationInfo.BusinessAddress.TownOrCity, organisationInfo.OrganisationAddress.TownOrCity);
            Assert.Equal(organisationInfo.BusinessAddress.CountyOrRegion,
                organisationInfo.OrganisationAddress.CountyOrRegion);
            Assert.Equal(organisationInfo.BusinessAddress.Postcode, organisationInfo.OrganisationAddress.Postcode);
            Assert.Equal(organisationInfo.BusinessAddress.Country, organisationInfo.OrganisationAddress.Country);
            Assert.Equal(organisationInfo.BusinessAddress.Telephone, organisationInfo.OrganisationAddress.Telephone);
            Assert.Equal(organisationInfo.BusinessAddress.Email, organisationInfo.OrganisationAddress.Email);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return dbHelper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("TEST Ltd")
            });
        }
    }
}

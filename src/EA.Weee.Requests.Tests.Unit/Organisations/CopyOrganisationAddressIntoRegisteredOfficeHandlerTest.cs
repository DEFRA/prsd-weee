namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
    using Xunit;

    public class CopyOrganisationAddressIntoRegisteredOfficeHandlerTest
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task CopyOrganisationAddressIntoRegisteredOfficeHandler_CopiedOrganisationAddressToRegisteredOffice()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new CopyOrganisationAddressIntoRegisteredOfficeHandler(context);

            var organisationId = await handler.HandleAsync(new CopyOrganisationAddressIntoRegisteredOffice(organisations.FirstOrDefault().Id));
            var organisationInfo = organisations.FirstOrDefault();

            Assert.NotNull(organisationInfo);
            Assert.NotNull(organisationId);
            Assert.Equal(organisationInfo.BusinessAddress.Address1, organisationInfo.OrganisationAddress.Address1);
            Assert.Equal(organisationInfo.BusinessAddress.Address2, organisationInfo.OrganisationAddress.Address2);
            Assert.Equal(organisationInfo.BusinessAddress.TownOrCity, organisationInfo.OrganisationAddress.TownOrCity);
            Assert.Equal(organisationInfo.BusinessAddress.CountyOrRegion, organisationInfo.OrganisationAddress.CountyOrRegion);
            Assert.Equal(organisationInfo.BusinessAddress.Postcode, organisationInfo.OrganisationAddress.Postcode);
            Assert.Equal(organisationInfo.BusinessAddress.Country, organisationInfo.OrganisationAddress.Country);
            Assert.Equal(organisationInfo.BusinessAddress.Telephone, organisationInfo.OrganisationAddress.Telephone);
            Assert.Equal(organisationInfo.BusinessAddress.Email, organisationInfo.OrganisationAddress.Email);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd"),
            });
        }
    }
}

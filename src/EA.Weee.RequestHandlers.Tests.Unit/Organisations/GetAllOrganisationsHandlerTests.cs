namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Tests.Core;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAllOrganisationsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();
        private readonly Guid currentUserID = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetAllOrganisationsHandler_ReturnsAllOrganisations()
        {
            var orgs = CreateOrgs();
            var handler = MakeTestGetAllOrganisationsHandler(orgs);

            var returnedOrgs = await handler.HandleAsync(new GetAllOrganisations());

            Assert.Equal(3, returnedOrgs.Count);
        }

        [Fact]
        public async Task GetAllOrganisationsHandler_Safely_ReturnsEmptyList()
        {
            var handler = MakeTestGetAllOrganisationsHandler(helper.GetAsyncEnabledDbSet(new Organisation[0]));

            var returnedOrgs = await handler.HandleAsync(new GetAllOrganisations());

            Assert.Equal(0, returnedOrgs.Count);
        }

        private DbSet<Organisation> CreateOrgs()
        {
            // Creates one of each type of organisation available in the system
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithDetails("Test Organisation", "Test Org", "CO12345678", OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(null, "Test Partnership", null, OrganisationType.Partnership, OrganisationStatus.Incomplete),
                orgHelper.GetOrganisationWithDetails("Test Sole Trader", "Test ST", "ST12345678", OrganisationType.SoleTraderOrIndividual, OrganisationStatus.Complete),
            });
        }

        private IWeeeAuthorization MakeTestUserAuthorization()
        {
            IWeeeAuthorization weeeAuthorization = A.Fake<IWeeeAuthorization>();
            //A.CallTo(() => userContext.UserId).Returns(currentUserID);
            return weeeAuthorization;
        }

        private OrganisationMap MakeTestOrganisationUserMap()
        {
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            return new OrganisationMap(addressMapper, contactMapper);
        }

        private GetAllOrganisationsHandler MakeTestGetAllOrganisationsHandler(DbSet<Organisation> orgs)
        {
            var context = A.Fake<WeeeContext>();
            var userContext = MakeTestUserAuthorization();
            var orgUsermapper = MakeTestOrganisationUserMap();

            A.CallTo(() => context.Organisations).Returns(orgs);

            return new GetAllOrganisationsHandler(userContext, context, orgUsermapper);
        }
    }
}

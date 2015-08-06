namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;
    using OrganisationType = Core.Organisations.OrganisationType;

    public class UpdateOrganisationTypeDetailsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task UpdateOrganisationTypeDetailsHandler_UpdateOrganisationTypeDetails_ReturnsUpdatedOrganisation()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new UpdateOrganisationTypeDetailsHandler(context);

            const string tradingName = "trading name";
            const string companyName = "company name";
            const string companyRegistrationNumber = "12345678";

            await
                handler.HandleAsync(new UpdateOrganisationTypeDetails(organisations.FirstOrDefault().Id,
                    OrganisationType.SoleTraderOrIndividual, companyName, tradingName, companyRegistrationNumber));

            var orgInfo = organisations.FirstOrDefault();

            Assert.NotNull(orgInfo);
            Assert.Equal(orgInfo.OrganisationType, Domain.Organisation.OrganisationType.SoleTraderOrIndividual);
            Assert.Equal(orgInfo.Name, companyName);
            Assert.Equal(orgInfo.TradingName, tradingName);
            Assert.Equal(orgInfo.CompanyRegistrationNumber, companyRegistrationNumber);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd")
            });
        }
    }
}

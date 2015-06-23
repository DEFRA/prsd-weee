namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
    using Xunit;

    public class VerifyOrganisationExistsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task VerifyOrganisationExistsHandler_OrgExists_ReturnsTrue()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(context);

            var exists = await handler.HandleAsync(new VerifyOrganisationExists(organisations.FirstOrDefault().Id));

            Assert.True(exists);
        }

        [Fact]
        public async Task VerifyOrganisationExistsHandler_OrgDoesntExist_ReturnsFalse()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(context);

            var exists = await handler.HandleAsync(new VerifyOrganisationExists(Guid.NewGuid()));

            Assert.False(exists);
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

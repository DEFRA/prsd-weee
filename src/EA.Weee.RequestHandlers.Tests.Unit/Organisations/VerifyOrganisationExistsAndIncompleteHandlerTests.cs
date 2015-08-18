namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
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

    public class VerifyOrganisationExistsAndIncompleteHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task VerifyOrganisationExistsAndIncompleteHandler_OrgExistsAndIncomplete_ReturnsTrue()
        {
            var organisations = MakeOrganisation();
            organisations.FirstOrDefault().OrganisationStatus = OrganisationStatus.Incomplete;

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(context);

            var exists =
                await handler.HandleAsync(new VerifyOrganisationExistsAndIncomplete(organisations.FirstOrDefault().Id));

            Assert.True(exists);
        }

        [Fact]
        public async Task VerifyOrganisationExistsAndIncompleteHandler_OrgExistsAndApproved_ReturnsFalse()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(context);

            var exists =
                await handler.HandleAsync(new VerifyOrganisationExistsAndIncomplete(organisations.FirstOrDefault().Id));

            Assert.False(exists);
        }

        [Fact]
        public async Task VerifyOrganisationExistsAndIncompleteHandler_OrgDoesNotExists_ReturnsFalse()
        {
            var organisations = MakeOrganisation();
            organisations.FirstOrDefault().OrganisationStatus = OrganisationStatus.Incomplete;

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(context);

            var exists = await handler.HandleAsync(new VerifyOrganisationExistsAndIncomplete(Guid.NewGuid()));

            Assert.False(exists);
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("TEST Ltd")
            });
        }
    }
}

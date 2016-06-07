namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
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

    public class VerifyOrganisationExistsAndIncompleteHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        [Fact]
        public async Task VerifyOrganisationExistsAndIncompleteHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(authorization, A.Dummy<WeeeContext>());
            var message = new VerifyOrganisationExistsAndIncomplete(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task VerifyOrganisationExistsAndIncompleteHandler_OrgExistsAndIncomplete_ReturnsTrue()
        {
            var organisations = MakeOrganisation();
            organisations.FirstOrDefault().OrganisationStatus = OrganisationStatus.Incomplete;

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(permissiveAuthorization, context);

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

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(permissiveAuthorization, context);

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

            var handler = new VerifyOrganisationExistsAndIncompleteHandler(permissiveAuthorization, context);

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

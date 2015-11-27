namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class VerifyOrganisationExistsHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserWithAllRights();

        [Fact]
        public async Task VerifyOrganisationExistsHandler_NotOrganisationOrInternalUser_ThrowsSecurityException()
        {
            var deniedAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new VerifyOrganisationExistsHandler(deniedAuthorization, A<WeeeContext>._);
            var message = new VerifyOrganisationExists(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task VerifyOrganisationExistsHandler_InternalUser_PassesSecurityCheck()
        {
            var internalAuthorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.Internal);

            var organisations = MakeOrganisation();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(internalAuthorization, context);
            var message = new VerifyOrganisationExists(Guid.NewGuid());

            await handler.HandleAsync(message);

            A.CallTo(() => context.Organisations).MustHaveHappened();
        }

        [Fact]
        public async Task VerifyOrganisationExistsHandler_OrganisationUser_PassesSecurityCheck()
        {
            var organisationAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var organisations = MakeOrganisation();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(organisationAuthorization, context);
            var message = new VerifyOrganisationExists(Guid.NewGuid());

            await handler.HandleAsync(message);

            A.CallTo(() => context.Organisations).MustHaveHappened();
        }

        [Fact]
        public async Task VerifyOrganisationExistsHandler_OrgExists_ReturnsTrue()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(permissiveAuthorization, context);

            var exists = await handler.HandleAsync(new VerifyOrganisationExists(organisations.FirstOrDefault().Id));

            Assert.True(exists);
        }

        [Fact]
        public async Task VerifyOrganisationExistsHandler_OrgDoesntExist_ReturnsFalse()
        {
            var organisations = MakeOrganisation();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new VerifyOrganisationExistsHandler(permissiveAuthorization, context);

            var exists = await handler.HandleAsync(new VerifyOrganisationExists(Guid.NewGuid()));

            Assert.False(exists);
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

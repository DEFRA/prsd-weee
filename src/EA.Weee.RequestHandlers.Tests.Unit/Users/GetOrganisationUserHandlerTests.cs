namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using System;
    using System.Data.Entity;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using Mappings;
    using RequestHandlers.Users;
    using Requests.Users;
    using Xunit;

    public class GetOrganisationUserHandlerTests
    {
        private readonly WeeeContext context = A.Fake<WeeeContext>();
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        private readonly Guid orgId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async void NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new GetOrganisationUserHandler(context, denyingAuthorization, new OrganisationUserMap(new OrganisationMap(new AddressMap(), new ContactMap()), new UserMap()));

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(new GetOrganisationUser(Guid.NewGuid(), Guid.NewGuid())));
        }

        [Fact]
        public async Task GetOrganisationUserHandler_ReturnsRequestedOrganisationUser()
        {
            var orgUsers = MakeOrganisationUsers();

            A.CallTo(() => context.OrganisationUsers).Returns(orgUsers);

            var handler = new GetOrganisationUserHandler(context, permissiveAuthorization, new OrganisationUserMap(new OrganisationMap(new AddressMap(), new ContactMap()), new UserMap()));

            var organisationUser = await handler.HandleAsync(new GetOrganisationUser(orgId, userId));

            Assert.NotNull(organisationUser);
            Assert.Equal(organisationUser.OrganisationId, orgId);
            Assert.Equal(organisationUser.UserId, userId.ToString());
        }

        private DbSet<OrganisationUser> MakeOrganisationUsers()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                CreateOrganisationUser(orgId, userId),
                CreateOrganisationUser(Guid.NewGuid(), Guid.NewGuid()),
            });
        }

        private static OrganisationUser CreateOrganisationUser(Guid orgId, Guid userId)
        {
            var orgUser = new OrganisationUser(userId, orgId, UserStatus.Active);
            return orgUser;
        }
    }
}

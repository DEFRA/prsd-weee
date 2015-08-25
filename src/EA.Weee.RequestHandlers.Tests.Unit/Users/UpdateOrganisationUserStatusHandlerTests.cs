namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using System;
    using System.Data.Entity;
    using System.Linq;
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
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateOrganisationUserStatusHandlerTests
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
            var handler = new UpdateOrganisationUserStatusHandler(context, denyingAuthorization);

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(new UpdateOrganisationUserStatus(Guid.NewGuid(), UserStatus.Active, Guid.NewGuid())));
        }

        [Fact]
        public async Task UpdateOrganisationUserStatusHandler_UpdateUserStatus_ReturnsUpdatedOrgUserId()
        {
            var orgUsers = MakeOrganisationUsers();

            A.CallTo(() => context.OrganisationUsers).Returns(orgUsers);

            var handler = new UpdateOrganisationUserStatusHandler(context, permissiveAuthorization);

            var organisationUserId = await handler.HandleAsync(new UpdateOrganisationUserStatus(orgId, UserStatus.Inactive, userId));

            Assert.NotNull(organisationUserId);
            Assert.Equal(organisationUserId, orgId);
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
            var orgUser = new OrganisationUser(userId, orgId, Domain.UserStatus.Active);
            return orgUser;
        }
    }
}

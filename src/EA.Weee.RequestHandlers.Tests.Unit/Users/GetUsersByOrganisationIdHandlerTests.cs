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

    public class GetUsersByOrganisationIdHandlerTests
    {
        private readonly WeeeContext context = A.Fake<WeeeContext>();
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly IWeeeAuthorization permissiveAuthorization = new AuthorizationBuilder().AllowOrganisationAccess().Build();
        private readonly IWeeeAuthorization denyingAuthorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
        private readonly Guid orgId = Guid.NewGuid();

        [Fact]
        public async void NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new GetUsersByOrganisationIdHandler(context, denyingAuthorization, new OrganisationUserMap(new OrganisationMap(new AddressMap(), new ContactMap()), new UserMap()));

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(new GetUsersByOrganisationId(Guid.NewGuid())));
        }

        [Fact]
        public async Task GetUsersByOrganisationIdHandler_ApprovalNumberNotExists_ReturnsFalse()
        {
            var orgUsers = MakeOrganisationUsers();

            A.CallTo(() => context.OrganisationUsers).Returns(orgUsers);

            var handler = new GetUsersByOrganisationIdHandler(context, permissiveAuthorization, new OrganisationUserMap(new OrganisationMap(new AddressMap(), new ContactMap()), new UserMap()));

            var organisationUsers = await handler.HandleAsync(new GetUsersByOrganisationId(orgId));

            Assert.NotNull(organisationUsers);
            Assert.Equal(organisationUsers.Count, 2);
        }

        private DbSet<OrganisationUser> MakeOrganisationUsers()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                CreateOrganisationUser(orgId),
                CreateOrganisationUser(orgId),
                CreateOrganisationUser(Guid.NewGuid()),
            });
        }

        private static OrganisationUser CreateOrganisationUser(Guid orgId)
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), orgId, UserStatus.Approved);
            return orgUser;
        }
    }
}

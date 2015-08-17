namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using Mappings;
    using RequestHandlers.Users;
    using Requests.Users;
    using Xunit;

    public class GetUsersByOrganisationIdHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly Guid orgId = Guid.NewGuid();

        [Fact]
        public async Task GetUsersByOrganisationIdHandler_ApprovalNumberNotExists_ReturnsFalse()
        {
            var orgUsers = MakeOrganisationUsers();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.OrganisationUsers).Returns(orgUsers);

            var handler = new GetUsersByOrganisationIdHandler(context, new OrganisationUserMap(new OrganisationMap(new AddressMap(), new ContactMap()), new UserMap()));

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

namespace EA.Weee.Requests.Tests.Unit.Account
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;
    using OrganisationUserStatus = Requests.Organisations.OrganisationUserStatus;

    public class GetOrganisationsByUserIdHandlerTest
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationUserHelper orgUserHelper = new OrganisationUserHelper();
        private readonly Guid userId = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_IsReturnApprovedOrganisationUser()
        {   
            var organisationUsers = MakeApprovedOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(), new[] { (int)OrganisationUserStatus.Approved }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(organisationUserInfo.OrganisationUserStatus, OrganisationUserStatus.Approved);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_IsReturnPendingOrganisationUser()
        {
            var organisationUsers = MakePendingOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(), new[] { (int)OrganisationUserStatus.Pending }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(organisationUserInfo.OrganisationUserStatus, OrganisationUserStatus.Pending);
        }

        private DbSet<OrganisationUser> MakeApprovedOrganisationUser(Guid userGuid)
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgUserHelper.GetApprovedOrganisationUser(userGuid)
            });
        }

        private DbSet<OrganisationUser> MakePendingOrganisationUser(Guid userGuid)
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgUserHelper.GetPendingOrganisationUser(userGuid)
            });
        }
    }
}

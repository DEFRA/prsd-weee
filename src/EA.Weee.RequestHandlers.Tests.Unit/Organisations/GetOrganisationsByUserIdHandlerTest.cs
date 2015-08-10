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
    using RequestHandlers.Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;
    using OrganisationUserStatus = Core.Organisations.OrganisationUserStatus;

    public class GetOrganisationsByUserIdHandlerTest
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationUserHelper orgUserHelper = new OrganisationUserHelper();
        private readonly Guid userId = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_RequestApprovedStatus_ReturnsApprovedOrganisationUsersOnly()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers =
                await
                    handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                        new[] { (int)OrganisationUserStatus.Approved }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(1, orgUsers.Count);
            Assert.Equal(organisationUserInfo.OrganisationUserStatus, OrganisationUserStatus.Approved);
        }

        [Fact]
        public async Task
            GetOrganisationsByUserIdHandler_RequestPendingAndRefusedStatus_ReturnsPendingAndRefusedOrganisationUsersOnly
            ()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers =
                await
                    handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                        new[] { (int)OrganisationUserStatus.Pending, (int)OrganisationUserStatus.Refused }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(2, orgUsers.Count);
            Assert.True(organisationUserInfo.OrganisationUserStatus == OrganisationUserStatus.Pending ||
                        organisationUserInfo.OrganisationUserStatus == OrganisationUserStatus.Refused);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestStatus_ReturnsAllOrganisationUsers()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(), new int[] { }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(3, orgUsers.Count);
        }

        private DbSet<OrganisationUser> MakeOrganisationUser(Guid userGuid)
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgUserHelper.GetOrganisationUser(userGuid, Domain.Organisation.OrganisationUserStatus.Approved),
                orgUserHelper.GetOrganisationUser(userGuid, Domain.Organisation.OrganisationUserStatus.Pending),
                orgUserHelper.GetOrganisationUser(userGuid, Domain.Organisation.OrganisationUserStatus.Refused)
            });
        }
    }
}

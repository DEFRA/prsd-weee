namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;

    public class GetOrganisationsByUserIdHandlerTest
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationUserHelper orgUserHelper = new OrganisationUserHelper();
        private readonly Guid userId = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_RequestApprovedUserStatus_ReturnsApprovedOrganisationUsersOnly()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers =
                await
                    handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                        new[] { (int)UserStatus.Active }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(1, orgUsers.Count);
            Assert.Equal(organisationUserInfo.UserStatus, UserStatus.Active);
        }

        [Fact]
        public async Task
            GetOrganisationsByUserIdHandler_RequestPendingAndRefusedUserStatus_ReturnsPendingAndRefusedOrganisationUsersOnly
            ()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers =
                await
                    handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                        new[] { (int)UserStatus.Pending, (int)UserStatus.Rejected }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(2, orgUsers.Count);
            Assert.True(organisationUserInfo.UserStatus == UserStatus.Pending ||
                        organisationUserInfo.UserStatus == UserStatus.Rejected);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestUserStatus_ReturnsAllOrganisationUsers()
        {
            var organisationUsers = MakeOrganisationUser(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(), new int[] { }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(3, orgUsers.Count);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestUserStatusCompletedOrganisation_ReturnsAllOrganisationUsersForCompletedOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                new int[] { },
                new int[] { (int)Core.Shared.OrganisationStatus.Complete }));

            Assert.Equal(2, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Active);
            Assert.True(orgUsers.Last().UserStatus == UserStatus.Rejected);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestUserStatusIncompleteOrganisation_ReturnsAllOrganisationUsersForIncompleteOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                new int[] { },
                new int[] { (int)Core.Shared.OrganisationStatus.Incomplete }));

            Assert.Equal(1, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Active);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_RequestRefusedUserStatusCompletedOrganisation_ReturnsRefusedOrganisationUsersForCompletedOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(userId);

            var context = A.Fake<WeeeContext>();
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            var orgUsermapper = new OrganisationUserMap(orgMapper, userMapper);

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            var handler = new GetOrganisationsByUserIdHandler(context, orgUsermapper);

            var orgUsers = await handler.HandleAsync(new GetOrganisationsByUserId(userId.ToString(),
                new int[] { (int)UserStatus.Rejected },
                new int[] { (int)Core.Shared.OrganisationStatus.Complete }));

            Assert.Equal(1, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Rejected);
        }

        private DbSet<OrganisationUser> MakeOrganisationUser(Guid userGuid)
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgUserHelper.GetOrganisationUser(userGuid, Domain.UserStatus.Active),
                orgUserHelper.GetOrganisationUser(userGuid, Domain.UserStatus.Pending),
                orgUserHelper.GetOrganisationUser(userGuid, Domain.UserStatus.Rejected)
            });
        }

        private DbSet<OrganisationUser> MakeOrganisationUserWithOrganisation(Guid userGuid)
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                MakeOrganisationUserWithOrganisation(userGuid, Domain.UserStatus.Active, Domain.Organisation.OrganisationStatus.Complete),
                MakeOrganisationUserWithOrganisation(userGuid, Domain.UserStatus.Active, Domain.Organisation.OrganisationStatus.Incomplete),
                MakeOrganisationUserWithOrganisation(userGuid, Domain.UserStatus.Rejected, Domain.Organisation.OrganisationStatus.Complete)
            });
        }

        private OrganisationUser MakeOrganisationUserWithOrganisation(Guid userGuid, Domain.UserStatus userStatus, Domain.Organisation.OrganisationStatus organisationStatus)
        {
            var fakeOrganisation = A.Fake<Organisation>();
            A.CallTo(() => fakeOrganisation.OrganisationStatus).Returns(organisationStatus);
            A.CallTo(() => fakeOrganisation.OrganisationType).Returns(A.Dummy<OrganisationType>());

            var fakeOrganisationUser = A.Fake<OrganisationUser>(x => x.WithArgumentsForConstructor(() => new OrganisationUser(userGuid, Guid.NewGuid(), userStatus)));
            A.CallTo(() => fakeOrganisationUser.Organisation).Returns(fakeOrganisation);

            return fakeOrganisationUser;
        }
    }
}

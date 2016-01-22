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
    using Prsd.Core.Domain;
    using RequestHandlers.Mappings;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUserOrganisationsByStatusHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationUserHelper orgUserHelper = new OrganisationUserHelper();
        private readonly Guid currentUserID = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetUserOrganisationsByStatusHandler_NoLinksToOrgs_ReturnsEmptyList()
        {
            var organisationUsers = MakeOrganisationUsers(new Guid("7EA27664-5286-4332-8628-AAE13FD9F10B"));
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers = await handler.HandleAsync(new GetUserOrganisationsByStatus(new int[] { }));

            Assert.Equal(0, orgUsers.Count);
        }

        [Fact]
        public async Task GetUserOrganisationsByStatusHandler_RequestApprovedUserStatus_ReturnsApprovedOrganisationUsersOnly()
        {
            var organisationUsers = MakeOrganisationUsers(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers =
                await
                    handler.HandleAsync(new GetUserOrganisationsByStatus(
                       new[] { (int)UserStatus.Active }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(1, orgUsers.Count);
            Assert.Equal(organisationUserInfo.UserStatus, UserStatus.Active);
        }

        [Fact]
        public async Task
            GetUserOrganisationsByStatusHandler_RequestPendingAndRefusedUserStatus_ReturnsPendingAndRefusedOrganisationUsersOnly
            ()
        {
            var organisationUsers = MakeOrganisationUsers(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers =
                await
                    handler.HandleAsync(new GetUserOrganisationsByStatus(new[] { (int)UserStatus.Pending, (int)UserStatus.Rejected }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(2, orgUsers.Count);
            Assert.True(organisationUserInfo.UserStatus == UserStatus.Pending ||
                        organisationUserInfo.UserStatus == UserStatus.Rejected);
        }

        [Fact]
        public async Task GetUserOrganisationsByStatusHandler_NoRequestUserStatus_ReturnsAllOrganisationUsers()
        {
            var organisationUsers = MakeOrganisationUsers(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers = await handler.HandleAsync(new GetUserOrganisationsByStatus(new int[] { }));
            var organisationUserInfo = orgUsers.FirstOrDefault();

            Assert.NotNull(organisationUserInfo);
            Assert.Equal(3, orgUsers.Count);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestUserStatusCompletedOrganisation_ReturnsAllOrganisationUsersForCompletedOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers = await handler.HandleAsync(new GetUserOrganisationsByStatus(
                new int[] { },
                new int[] { (int)Core.Shared.OrganisationStatus.Complete }));

            Assert.Equal(2, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Active);
            Assert.True(orgUsers.Last().UserStatus == UserStatus.Rejected);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_NoRequestUserStatusIncompleteOrganisation_ReturnsAllOrganisationUsersForIncompleteOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers = await handler.HandleAsync(new GetUserOrganisationsByStatus(
                new int[] { },
                new int[] { (int)Core.Shared.OrganisationStatus.Incomplete }));

            Assert.Equal(1, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Active);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdHandler_RequestRefusedUserStatusCompletedOrganisation_ReturnsRefusedOrganisationUsersForCompletedOrganisation()
        {
            var organisationUsers = MakeOrganisationUserWithOrganisation(currentUserID);
            var handler = MakeTestGetUserOrganisationsByStatusHandler(organisationUsers);

            var orgUsers = await handler.HandleAsync(new GetUserOrganisationsByStatus(
                new int[] { (int)UserStatus.Rejected },
                new int[] { (int)Core.Shared.OrganisationStatus.Complete }));

            Assert.Equal(1, orgUsers.Count);
            Assert.True(orgUsers.First().UserStatus == UserStatus.Rejected);
        }

        private DbSet<OrganisationUser> MakeOrganisationUsers(Guid userID)
        {
            Guid otherUserID = new Guid("4462D664-5286-4332-8628-AAE13FD984CA");

            return helper.GetAsyncEnabledDbSet(new[]
            {   
                orgUserHelper.GetOrganisationUser(userID, Domain.User.UserStatus.Active),
                orgUserHelper.GetOrganisationUser(userID, Domain.User.UserStatus.Pending),
                orgUserHelper.GetOrganisationUser(userID, Domain.User.UserStatus.Rejected),
                orgUserHelper.GetOrganisationUser(otherUserID, Domain.User.UserStatus.Active)
            });
        }

        private DbSet<OrganisationUser> MakeOrganisationUserWithOrganisation(Guid userGuid)
        {
            Guid otherUserID = new Guid("4462D664-5286-4332-8628-AAE13FD984CA");

            return helper.GetAsyncEnabledDbSet(new[]
            {
                MakeOrganisationUserWithOrganisation(userGuid, Domain.User.UserStatus.Active, Domain.Organisation.OrganisationStatus.Complete),
                MakeOrganisationUserWithOrganisation(userGuid, Domain.User.UserStatus.Active, Domain.Organisation.OrganisationStatus.Incomplete),
                MakeOrganisationUserWithOrganisation(userGuid, Domain.User.UserStatus.Rejected, Domain.Organisation.OrganisationStatus.Complete),
                MakeOrganisationUserWithOrganisation(otherUserID, Domain.User.UserStatus.Active, Domain.Organisation.OrganisationStatus.Complete)
            });
        }

        private OrganisationUser MakeOrganisationUserWithOrganisation(Guid userGuid, Domain.User.UserStatus userStatus, Domain.Organisation.OrganisationStatus organisationStatus)
        {
            var fakeOrganisation = A.Fake<Organisation>();
            A.CallTo(() => fakeOrganisation.OrganisationStatus).Returns(organisationStatus);
            A.CallTo(() => fakeOrganisation.OrganisationType).Returns(A.Dummy<OrganisationType>());

            var fakeOrganisationUser = A.Fake<OrganisationUser>(x => x.WithArgumentsForConstructor(() => new OrganisationUser(userGuid, Guid.NewGuid(), userStatus)));
            A.CallTo(() => fakeOrganisationUser.Organisation).Returns(fakeOrganisation);

            return fakeOrganisationUser;
        }

        private IUserContext MakeTestUserContext()
        {
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(currentUserID);
            return userContext;
        }

        private OrganisationUserMap MakeTestOrganisationUserMap()
        {
            var addressMapper = new AddressMap();
            var contactMapper = new ContactMap();
            var userMapper = new UserMap();
            var orgMapper = new OrganisationMap(addressMapper, contactMapper);
            return new OrganisationUserMap(orgMapper, userMapper);
        }

        private GetUserOrganisationsByStatusHandler MakeTestGetUserOrganisationsByStatusHandler(DbSet<OrganisationUser> organisationUsers)
        {
            var context = A.Fake<WeeeContext>();
            var userContext = MakeTestUserContext();
            var orgUsermapper = MakeTestOrganisationUserMap();

            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            return new GetUserOrganisationsByStatusHandler(context, userContext, orgUsermapper);
        }
    }
}

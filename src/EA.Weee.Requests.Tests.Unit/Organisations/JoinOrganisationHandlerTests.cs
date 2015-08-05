namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;

    public class JoinOrganisationHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();
        private readonly UserHelper userHelper = new UserHelper();

        private readonly Guid userGuid = Guid.NewGuid();

        [Fact]
        public async Task JoinOrganisationHandler_HappyPath_ReturnsOrganisationGuid()
        {
            var context = SetupFakeWeeeContext();
            var userContext = SetupFakeUserContext();

            var handler = new JoinOrganisationHandler(context, userContext);

            var organisationWeWillJoin = await context.Organisations.FirstOrDefaultAsync();

            await handler.HandleAsync(new JoinOrganisation(organisationWeWillJoin.Id));

            A.CallTo(() => context.OrganisationUsers.Add(A<OrganisationUser>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task JoinOrganisationHandler_HappyPath_SetsOrganisationUserStatusToPending()
        {
            var context = SetupFakeWeeeContext();
            var userContext = SetupFakeUserContext();

            OrganisationUser addedOrganisationUser = null;
            A.CallTo(() => context.OrganisationUsers.Add(A<OrganisationUser>._))
                .Invokes((OrganisationUser ou) => addedOrganisationUser = ou);

            var handler = new JoinOrganisationHandler(context, userContext);

            var organisationWeWillJoin = context.Organisations.FirstOrDefault();

            await handler.HandleAsync(new JoinOrganisation(organisationWeWillJoin.Id));

            Assert.Equal(OrganisationUserStatus.Pending, addedOrganisationUser.UserStatus);
        }

        [Fact]
        public async Task JoinOrganisationHandler_NoSuchUser_ThrowsArgumentException()
        {
            var context = SetupFakeWeeeContext();
            var userContext = SetupFakeUserContext();

            var guidThatDoesntExistInDatabase = Guid.NewGuid();
            A.CallTo(() => userContext.UserId).Returns(guidThatDoesntExistInDatabase);

            var handler = new JoinOrganisationHandler(context, userContext);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () => await handler.HandleAsync(new JoinOrganisation(Guid.NewGuid())));
        }

        [Fact]
        public async Task JoinOrganisationHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var context = SetupFakeWeeeContext();
            var userContext = SetupFakeUserContext();

            var handler = new JoinOrganisationHandler(context, userContext);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () => await handler.HandleAsync(new JoinOrganisation(Guid.NewGuid())));
        }

        private WeeeContext SetupFakeWeeeContext()
        {
            var context = A.Fake<WeeeContext>();

            var organisations = MakeOrganisation();
            var users = MakeUser();
            var organisationUsers = helper.GetAsyncEnabledDbSet(new OrganisationUser[] { });

            A.CallTo(() => context.Organisations).Returns(organisations);
            A.CallTo(() => context.Users).Returns(users);
            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            return context;
        }

        private IUserContext SetupFakeUserContext()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(userGuid);

            return userContext;
        }

        private DbSet<Organisation> MakeOrganisation()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd")
            });
        }

        private DbSet<User> MakeUser()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(userGuid)
            });
        }
    }
}

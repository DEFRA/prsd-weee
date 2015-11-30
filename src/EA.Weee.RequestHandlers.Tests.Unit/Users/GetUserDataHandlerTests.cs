namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Security;
    using RequestHandlers.Users;
    using Requests.Users;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUserDataHandlerTests
    {
        private readonly DbContextHelper helper;
        private readonly UserHelper userHelper;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization weeeAuthorization;
        
        public GetUserDataHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            userContext = A.Fake<IUserContext>();

            userHelper = new UserHelper();
            helper = new DbContextHelper();
        }

        [Fact]
        public async Task GetUserDataHandler_NotRequestingCurrentUser_AndUserIsNotAdmin_ThrowsException()
        {
            var otherUserId = Guid.NewGuid().ToString();

            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());

            A.CallTo(() => weeeAuthorization.CheckCanAccessInternalArea())
                .Returns(false);

            await
                Assert.ThrowsAsync<UnauthorizedAccessException>(
                    async () => await GetUserDataHandler().HandleAsync(new GetUserData(otherUserId)));
        }

        [Fact]
        public async Task GetUserDataHandler_RequestingCurrentUser_ButUserIsNotAdmin_ReturnsUserData()
        {
            var userId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            A.CallTo(() => context.Users).Returns(helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(userId)
            }));

            A.CallTo(() => weeeAuthorization.CheckCanAccessInternalArea())
                .Returns(false);

            var userData = await GetUserDataHandler().HandleAsync(new GetUserData(userId.ToString()));

            Assert.Equal(userData.Id, userId.ToString());
        }

        [Fact]
        public async Task GetUserDataHandler_NotRequestingCurrentUser_ButUserIsAdmin_ReturnsUserData()
        {
            var requestUserId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId)
                .Returns(currentUserId);

            A.CallTo(() => context.Users).Returns(helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(requestUserId)
            }));

            A.CallTo(() => weeeAuthorization.CheckCanAccessInternalArea())
                .Returns(true);

            var userData = await GetUserDataHandler().HandleAsync(new GetUserData(requestUserId.ToString()));

            Assert.Equal(userData.Id, requestUserId.ToString());
        }

        [Fact]
        public async Task GetUserDataHandler_RequestingCurrentUser_AndUserIsAdmin_ReturnsUserData()
        {
            var userId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            A.CallTo(() => context.Users).Returns(helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(userId)
            }));

            A.CallTo(() => weeeAuthorization.CheckCanAccessInternalArea())
                .Returns(true);

            var userData = await GetUserDataHandler().HandleAsync(new GetUserData(userId.ToString()));

            Assert.Equal(userData.Id, userId.ToString());
        }

        private GetUserDataHandler GetUserDataHandler()
        {
            return new GetUserDataHandler(context, userContext, weeeAuthorization);
        }
    }
}

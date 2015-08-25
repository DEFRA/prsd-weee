namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using FakeItEasy;
    using Helpers;
    using Prsd.Core.Domain;
    using RequestHandlers.Users;
    using Requests.Users;
    using Xunit;

    public class GetUserDataHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly UserHelper userHelper = new UserHelper();
        private readonly Guid currentUserID = new Guid("012F9664-5286-433A-8628-AAE13FD1C2F5");

        [Fact]
        public async Task GetUserDataHandler_NotRequestingCurrentUser_ThrowsException()
        {
            var otherUserID = "7EA27664-5286-4332-8628-AAE13FD9F10B";

            var context = A.Fake<WeeeContext>();
            var handler = new GetUserDataHandler(context, MakeTestUserContext());

            await
                Assert.ThrowsAsync<UnauthorizedAccessException>(
                    async () => await handler.HandleAsync(new GetUserData(otherUserID)));
        }

        [Fact]
        public async Task GetUserDataHandler_RequestingCurrentUser_ReturnsUserData()
        {
            var context = A.Fake<WeeeContext>();
            var handler = new GetUserDataHandler(context, MakeTestUserContext());

            A.CallTo(() => context.Users).Returns(MakeUser());

            var userData = await handler.HandleAsync(new GetUserData(currentUserID.ToString()));

            Assert.Equal(userData.Id, currentUserID.ToString());
        }

        private IUserContext MakeTestUserContext()
        {
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(currentUserID);
            return userContext;
        }

        private DbSet<User> MakeUser()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(currentUserID)
            });
        }
    }
}

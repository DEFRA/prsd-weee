namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class UpdateUserHandlerTests
    {   
        private readonly DbContextHelper helper;
        private readonly UserHelper userHelper;

        public UpdateUserHandlerTests()
        {
            userHelper = new UserHelper();
            helper = new DbContextHelper();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void HandleAsync_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrage
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            WeeeContext context = A.Fake<WeeeContext>();
            var handler = new UpdateUserHandler(context, authorization);
            var request = new UpdateUser(Guid.NewGuid().ToString(), "TestFirstName", "TestLastName");

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownUserId_ThrowsArgumentException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid userId = new Guid("81e0c989-43f9-4719-94ad-45a11c2ef368");

            WeeeContext context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Users).Returns(helper.GetAsyncEnabledDbSet(new[]
            {
                userHelper.GetUser(userId)
            }));

            var handler = new UpdateUserHandler(context, authorization);
            var request = new UpdateUser(Guid.NewGuid().ToString(), "TestFirstName", "TestLastName");

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_HappyPath_UpdatedUserInformation()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid userId = new Guid("81e0c989-43f9-4719-94ad-45a11c2ef368");

            WeeeContext context = A.Fake<WeeeContext>();

            var user = userHelper.GetUser(userId);

            A.CallTo(() => context.Users).Returns(helper.GetAsyncEnabledDbSet(new[]
            {
                user
            }));

            var handler = new UpdateUserHandler(context, authorization);
            var request = new UpdateUser(userId.ToString(), "Test", "Test");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(user.FirstName, "Test");
            Assert.Equal(user.Surname, "Test");
        }
    }
}

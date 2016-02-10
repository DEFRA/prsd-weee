namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Security;
    using EA.Weee.DataAccess.Identity;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
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
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new UpdateUserHandler(authorization, userManager);

            var request = new UpdateUser(Guid.NewGuid().ToString(), "TestFirstName", "TestLastName");

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new UpdateUserHandler(authorization, userManager);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(A<UpdateUser>._);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownUserId_ThrowsArgumentException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();
            A.CallTo(() => userManager.FindByIdAsync("722F0857-EE0A-4460-AA7B-0E8EF5B5C8A1")).Returns((ApplicationUser)null);

            var handler = new UpdateUserHandler(authorization, userManager);
            var request = new UpdateUser("722F0857-EE0A-4460-AA7B-0E8EF5B5C8A1", "TestFirstName", "TestLastName");

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

            ApplicationUser user = new ApplicationUser();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();
            A.CallTo(() => userManager.FindByIdAsync("722F0857-EE0A-4460-AA7B-0E8EF5B5C8A1")).Returns(user);
            A.CallTo(() => userManager.UpdateAsync(user)).Returns(IdentityResult.Success);

            var handler = new UpdateUserHandler(authorization, userManager);

            var request = new UpdateUser("722F0857-EE0A-4460-AA7B-0E8EF5B5C8A1", "Test", "Test");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(user.FirstName, "Test");
            Assert.Equal(user.Surname, "Test");
            A.CallTo(() => userManager.UpdateAsync(user)).MustHaveHappened();
        }
    }
}

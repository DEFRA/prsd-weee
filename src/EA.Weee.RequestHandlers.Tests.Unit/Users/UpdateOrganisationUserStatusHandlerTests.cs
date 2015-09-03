namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using DataAccess;
    using Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Users;
    using Requests.Users;
    using System;
    using System.Data.Entity;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateOrganisationUserStatusHandlerTests
    {
        [Fact]
        public async Task HandleAsync_HappyPath_ReturnsZero()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");
            Guid userId = new Guid("E69D3021-FD73-423B-A70C-562888C34D48");
            Guid organisationId = new Guid("BF3ACC9C-0A5B-4C32-9E6A-95FC7FE4CAF8");
            
            OrganisationUser organisationUser = new OrganisationUser(
                userId, organisationId, EA.Weee.Domain.UserStatus.Pending);
            // Note: Unable to mock the getter for organisationUser.Id, so it is being left as Guid.Empty.

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns(organisationUser);

            var handler = new UpdateOrganisationUserStatusHandler(context, authorization);
            var request = new UpdateOrganisationUserStatus(organisationUserId, UserStatus.Active);
            
            // Act
            int result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(EA.Weee.Domain.UserStatus.Active, organisationUser.UserStatus);
            Assert.Equal(0, result);            
        }

        [Fact]
        [Trait("Authorization", "OrganisationAccess")]
        public async Task HandleAsync_NotOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");
            OrganisationUser organisationUser = A.Fake<OrganisationUser>();
            // Note: Unable to mock the getter for organisationUser.Id, so it is being left as Guid.Empty.

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns(organisationUser);

            var handler = new UpdateOrganisationUserStatusHandler(context, authorization);
            var request = new UpdateOrganisationUserStatus(organisationUserId, UserStatus.Active);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownOrganisationUserId_ThrowsException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns((OrganisationUser)null);

            var handler = new UpdateOrganisationUserStatusHandler(context, authorization);
            var request = new UpdateOrganisationUserStatus(organisationUserId, UserStatus.Active);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }
    }
}

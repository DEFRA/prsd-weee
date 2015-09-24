namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using System;
    using System.Threading.Tasks;
    using Domain.Admin;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Xunit;
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateCompetentAuthorityUserStatusHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithUnknownCompetentAuthorityUserId_ThrowsException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid competentAuthorityUserId = new Guid("81e0c989-43f9-4719-94ad-45a11c2ef368");

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.CompetentAuthorityUsers.FindAsync(competentAuthorityUserId)).Returns((CompetentAuthorityUser)null);

            var handler = new UpdateCompetentAuthorityUserStatusHandler(context, authorization);
            var request = new UpdateCompetentAuthorityUserStatus(competentAuthorityUserId, UserStatus.Active);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task HandleAsync_HappyPath_UpdatedUserStatus()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid competentAuthorityUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");
            Guid userId = new Guid("E69D3021-FD73-423B-A70C-562888C34D48");
            Guid competentAuthorityId = new Guid("BF3ACC9C-0A5B-4C32-9E6A-95FC7FE4CAF8");

            CompetentAuthorityUser competentAuthorityUser = new CompetentAuthorityUser(
                userId.ToString(), competentAuthorityId, EA.Weee.Domain.UserStatus.Pending);

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.CompetentAuthorityUsers.FindAsync(competentAuthorityUserId)).Returns(competentAuthorityUser);

            var handler = new UpdateCompetentAuthorityUserStatusHandler(context, authorization);
            var request = new UpdateCompetentAuthorityUserStatus(competentAuthorityUserId, UserStatus.Active);

            // Act
            await handler.HandleAsync(request);

            // Assert
            Assert.Equal(EA.Weee.Domain.UserStatus.Active, competentAuthorityUser.UserStatus);
        }
    }
}

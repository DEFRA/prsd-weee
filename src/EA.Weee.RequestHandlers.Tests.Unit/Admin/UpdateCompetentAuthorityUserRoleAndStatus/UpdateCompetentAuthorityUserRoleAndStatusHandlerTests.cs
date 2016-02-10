namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Security;
    using Domain.Admin;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;
    using Role = Domain.Security.Role;
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateCompetentAuthorityUserRoleAndStatusHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(A.Dummy<IUserContext>(), A.Dummy<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>(),
                authorization);

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<UpdateCompetentAuthorityUserRoleAndStatus>()));
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(A.Dummy<IUserContext>(), A.Dummy<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>(),
                authorization);

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<UpdateCompetentAuthorityUserRoleAndStatus>()));
        }

        [Fact]
        public async Task HandleAsync_UserDoesNotExist_ThrowInvalidOperationException()
        {
            var dataAccess = A.Fake<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>();
            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(A<Guid>._))
                .Returns((CompetentAuthorityUser)null);

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(A.Dummy<IUserContext>(), dataAccess, A.Dummy<IWeeeAuthorization>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(A.Dummy<UpdateCompetentAuthorityUserRoleAndStatus>()));
        }

        [Fact]
        public async Task HandleAsync_UserExists_ButIsCurrentUser_ThrowInvalidOperationException()
        {
            var userId = Guid.NewGuid();
            var competentAuthorityUserId = Guid.NewGuid();
            var competentAuthorityUser = new CompetentAuthorityUser(userId.ToString(), Guid.NewGuid(), Domain.User.UserStatus.Pending, A.Dummy<Role>());

            var dataAccess = A.Fake<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>();
            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(competentAuthorityUserId))
                .Returns(competentAuthorityUser);

            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(userContext, dataAccess, A.Dummy<IWeeeAuthorization>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(
                new UpdateCompetentAuthorityUserRoleAndStatus(competentAuthorityUserId, A.Dummy<UserStatus>(), A<string>._)));
        }

        [Fact]
        public async Task HandleAsync_InvalidRoleName_ThrowInvalidOperationException()
        {
            var dataAccess = A.Fake<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>();
            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(A<Guid>._))
                .Returns(A.Dummy<CompetentAuthorityUser>());

            A.CallTo(() => dataAccess.GetRoleOrDefaultAsync(A<string>._))
                .Returns((Role)null);

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(A.Dummy<IUserContext>(), dataAccess, A.Dummy<IWeeeAuthorization>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(A.Dummy<UpdateCompetentAuthorityUserRoleAndStatus>()));
        }

        [Fact]
        public async Task HandleAsync_UpdatesUserRoleAndStatus()
        {
            var userId = Guid.NewGuid();
            var roleName = "RoleName";

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            var role = A.Fake<Role>();
            var userStatus = UserStatus.Active;

            var dataAccess = A.Fake<IUpdateCompetentAuthorityUserRoleAndStatusDataAccess>();
            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(userId))
                .Returns(competentAuthorityUser);

            A.CallTo(() => dataAccess.GetRoleOrDefaultAsync(roleName))
                .Returns(role);

            var handler = new UpdateCompetentAuthorityUserRoleAndStatusHandler(A.Dummy<IUserContext>(), dataAccess, A.Dummy<IWeeeAuthorization>());

            await handler.HandleAsync(new UpdateCompetentAuthorityUserRoleAndStatus(userId, userStatus, roleName));

            A.CallTo(() => dataAccess.UpdateUserRoleAndStatus(competentAuthorityUser, role, userStatus))
                .MustHaveHappened();
        }
    }
}

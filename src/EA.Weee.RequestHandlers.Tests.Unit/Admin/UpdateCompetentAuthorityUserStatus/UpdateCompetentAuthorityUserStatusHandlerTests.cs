namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.UpdateCompetentAuthorityUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Domain.Admin;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Xunit;
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateCompetentAuthorityUserStatusHandlerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IUpdateCompetentAuthorityUserStatusDataAccess dataAccess;
        private readonly IUserContext userContext;

        public UpdateCompetentAuthorityUserStatusHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>(); // Only throws exceptions when authorization errors, so will pass authorization by default
            dataAccess = A.Fake<IUpdateCompetentAuthorityUserStatusDataAccess>();
            userContext = A.Fake<IUserContext>();
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Active)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task UserDoesNotExist_ThrowsException(UserStatus userStatus)
        {
            var competentAuthorityUserId = Guid.NewGuid();

            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(competentAuthorityUserId))
                .Returns((CompetentAuthorityUser)null);

            await Assert.ThrowsAnyAsync<Exception>(
                () =>
                    UpdateCompetentAuthorityUserStatusHandler()
                        .HandleAsync(new UpdateCompetentAuthorityUserStatus(competentAuthorityUserId, userStatus)));
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Active)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task UserExists_ButIsCurrentUser_ShouldThrowInvalidOperationException(UserStatus userStatus)
        {
            var userId = Guid.NewGuid();
            var competentAuthorityUserId = Guid.NewGuid();
            var competentAuthorityUser = CompetentAuthorityUser(userId);

            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(competentAuthorityUserId))
                .Returns(competentAuthorityUser);

            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            await Assert.ThrowsAnyAsync<InvalidOperationException>(() => UpdateCompetentAuthorityUserStatusHandler()
                .HandleAsync(new UpdateCompetentAuthorityUserStatus(competentAuthorityUserId, userStatus)));
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Active)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task UserExists_AndIsNotCurrentUser_ShouldVerifyAuthorization_BeforeChangingUserStatus(UserStatus userStatus)
        {
            var userId = Guid.NewGuid();
            var competentAuthorityUserId = Guid.NewGuid();
            var competentAuthorityUser = CompetentAuthorityUser(Guid.NewGuid());

            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(competentAuthorityUserId))
                .Returns(competentAuthorityUser);

            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            using (var scope = Fake.CreateScope())
            {
                await
                    UpdateCompetentAuthorityUserStatusHandler()
                        .HandleAsync(new UpdateCompetentAuthorityUserStatus(competentAuthorityUserId, userStatus));

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                        .MustHaveHappened(Repeated.Exactly.Once);

                    A.CallTo(() => dataAccess.UpdateCompetentAuthorityUserStatus(competentAuthorityUser, userStatus))
                        .MustHaveHappened(Repeated.Exactly.Once);
                }
            }
        }

        private UpdateCompetentAuthorityUserStatusHandler UpdateCompetentAuthorityUserStatusHandler()
        {
            return new UpdateCompetentAuthorityUserStatusHandler(userContext, dataAccess, weeeAuthorization);
        }

        private CompetentAuthorityUser CompetentAuthorityUser(Guid? userId = null)
        {
            if (userId.HasValue)
            {
                return new CompetentAuthorityUser(userId.Value.ToString(), Guid.NewGuid(), Domain.User.UserStatus.Pending);
            }

            return A.Fake<CompetentAuthorityUser>();
        }
    }
}

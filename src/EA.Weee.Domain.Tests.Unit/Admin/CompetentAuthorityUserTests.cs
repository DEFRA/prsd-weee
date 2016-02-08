namespace EA.Weee.Domain.Tests.Unit.Admin
{
    using System;
    using Domain.Admin;
    using FakeItEasy;
    using Security;
    using User;
    using Xunit;

    public class CompetentAuthorityUserTests
    {
        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusIsActive_UpdateToRejected_ThrowsException()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Active, A.Dummy<Role>());

            Assert.Throws<InvalidOperationException>(() => competentAuthorityUser.UpdateUserStatus(UserStatus.Rejected));
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusIsPending_UpdateToInactive_ThrowsException()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Pending, A.Dummy<Role>());

            Assert.Throws<InvalidOperationException>(() => competentAuthorityUser.UpdateUserStatus(UserStatus.Inactive));
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusIsInactive_UpdateToPending_ThrowsException()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Inactive, A.Dummy<Role>());

            Assert.Throws<InvalidOperationException>(() => competentAuthorityUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusIsActive_UpdateToPending_ThrowsException()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Active, A.Dummy<Role>());

            Assert.Throws<InvalidOperationException>(() => competentAuthorityUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusRejected_UpdateToPending_ThrowsException()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Rejected, A.Dummy<Role>());

            Assert.Throws<InvalidOperationException>(() => competentAuthorityUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusActive_UpdateToInactive_UserStatusUpdateToInactive()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Active, A.Dummy<Role>());
            competentAuthorityUser.UpdateUserStatus(UserStatus.Inactive);

            Assert.Equal(competentAuthorityUser.UserStatus, UserStatus.Inactive);
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusPending_UpdateToActive_UserStatusUpdateToActive()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Pending, A.Dummy<Role>());
            competentAuthorityUser.UpdateUserStatus(UserStatus.Active);

            Assert.Equal(competentAuthorityUser.UserStatus, UserStatus.Active);
        }

        [Fact]
        public void UpdateCompetentAuthorityUserStatus_UserStatusRejected_UpdateToActive_UserStatusUpdateToActive()
        {
            var competentAuthorityUser = new CompetentAuthorityUser(Guid.NewGuid().ToString(), Guid.NewGuid(), UserStatus.Rejected, A.Dummy<Role>());
            competentAuthorityUser.UpdateUserStatus(UserStatus.Active);

            Assert.Equal(competentAuthorityUser.UserStatus, UserStatus.Active);
        }
    }
}

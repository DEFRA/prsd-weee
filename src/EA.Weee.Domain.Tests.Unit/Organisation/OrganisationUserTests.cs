namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using Domain.Organisation;
    using System;
    using Xunit;

    public class OrganisationUserTests
    {
        [Fact]
        public void UpdateUserStatus_UserStatusIsActive_UpdateToRejected_ThrowsException()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Active);

            Assert.Throws<InvalidOperationException>(() => orgUser.UpdateUserStatus(UserStatus.Rejected));
        }

        [Fact]
        public void UpdateUserStatus_UserStatusIsPending_UpdateToInactive_ThrowsException()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Pending);

            Assert.Throws<InvalidOperationException>(() => orgUser.UpdateUserStatus(UserStatus.Inactive));
        }

        [Fact]
        public void UpdateUserStatus_UserStatusIsInactive_UpdateToPending_ThrowsException()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Inactive);

            Assert.Throws<InvalidOperationException>(() => orgUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateUserStatus_UserStatusIsActive_UpdateToPending_ThrowsException()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Active);

            Assert.Throws<InvalidOperationException>(() => orgUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateUserStatus_UserStatusRejected_UpdateToPending_ThrowsException()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Rejected);

            Assert.Throws<InvalidOperationException>(() => orgUser.UpdateUserStatus(UserStatus.Pending));
        }

        [Fact]
        public void UpdateUserStatus_UserStatusActive_UpdateToInactive_UserStatusUpdateToInactive()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Active);
            orgUser.UpdateUserStatus(UserStatus.Inactive);

            Assert.Equal(orgUser.UserStatus, UserStatus.Inactive);
        }

        [Fact]
        public void UpdateUserStatus_UserStatusPending_UpdateToActive_UserStatusUpdateToActive()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Pending);
            orgUser.UpdateUserStatus(UserStatus.Active);

            Assert.Equal(orgUser.UserStatus, UserStatus.Active);
        }

        [Fact]
        public void UpdateUserStatus_UserStatusRejected_UpdateToActive_UserStatusUpdateToActive()
        {
            var orgUser = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Rejected);
            orgUser.UpdateUserStatus(UserStatus.Active);

            Assert.Equal(orgUser.UserStatus, UserStatus.Active);
        }
    }
}

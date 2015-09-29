namespace EA.Weee.RequestHandlers.Tests.Unit.Users.UpdateOrganisationUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Security;
    using RequestHandlers.Users.UpdateOrganisationUserStatus;
    using Requests.Users;
    using Xunit;
    using UserStatus = Core.Shared.UserStatus;

    public class UpdateOrganisationUserStatusHandlerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IUpdateOrganisationUserStatusDataAccess dataAccess;

        public UpdateOrganisationUserStatusHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>(); // Only throws exceptions when authorization errors, so will pass authorization by default
            dataAccess = A.Fake<IUpdateOrganisationUserStatusDataAccess>();
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Active)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task UserDoesNotExist_ThrowsException(UserStatus userStatus)
        {
            var organisationUserId = Guid.NewGuid();

            A.CallTo(() => dataAccess.GetOrganisationUser(organisationUserId))
                .Returns((OrganisationUser)null);

            await Assert.ThrowsAnyAsync<Exception>(
                () =>
                    UpdateOrganisationUserStatusHandler()
                        .HandleAsync(new UpdateOrganisationUserStatus(organisationUserId, userStatus)));
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Active)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async Task OrganisationUserExists_ShouldVerifyAuthorization_BeforeChangingOrgansiationUserStatus(UserStatus userStatus)
        {
            var organisationId = Guid.NewGuid();
            var organisationUserId = Guid.NewGuid();
            var organisationUser = OrganisationUser(userStatus, organisationId);

            A.CallTo(() => dataAccess.GetOrganisationUser(organisationUserId))
                .Returns(organisationUser);

            using (var scope = Fake.CreateScope())
            {
                await
                    UpdateOrganisationUserStatusHandler()
                        .HandleAsync(new UpdateOrganisationUserStatus(organisationUserId, userStatus));

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => weeeAuthorization.EnsureInternalOrOrganisationAccess(A<Guid>._))
                        .MustHaveHappened(Repeated.Exactly.Once);

                    A.CallTo(() => dataAccess.ChangeOrganisationUserStatus(organisationUser, userStatus))
                        .MustHaveHappened(Repeated.Exactly.Once);
                }
            }
        }

        private UpdateOrganisationUserStatusHandler UpdateOrganisationUserStatusHandler()
        {
            return new UpdateOrganisationUserStatusHandler(weeeAuthorization, dataAccess);
        }

        private OrganisationUser OrganisationUser(UserStatus userStatus = UserStatus.Pending, Guid? organisationId = null)
        {
            if (!organisationId.HasValue)
            {
                return A.Fake<OrganisationUser>();
            }

            return new OrganisationUser(Guid.NewGuid(), organisationId.Value, userStatus.ToDomainEnumeration<Domain.UserStatus>());
        }
    }
}

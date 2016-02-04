namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUserDataHandlerTests
    {
        private readonly Guid orgUserId = Guid.NewGuid();

        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGetManageUserDataAccess dataAccess;

        public GetUserDataHandlerTests()
        {
            userContext = A.Fake<IUserContext>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetManageUserDataAccess>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void GetUserDataHandler_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrage
            IGetManageUserDataAccess dataAccess = A.Fake<IGetManageUserDataAccess>();
            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(Guid.NewGuid())).Returns(new ManageUserData());
            A.CallTo(() => dataAccess.GetOrganisationUser(Guid.NewGuid())).Returns(new ManageUserData());

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            GetUserDataHandler handler = new GetUserDataHandler(userContext, authorization, dataAccess);

            GetUserData request = new GetUserData(Guid.NewGuid());

            // Act
            Func<Task<ManageUserData>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async void GetUserDataHandler_RequestingUserData_ReturnsCorrectUser()
        {
            // Arrage
            IGetManageUserDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetUserDataHandler handler = new GetUserDataHandler(userContext, authorization, dataAccess);

            GetUserData request = new GetUserData(orgUserId);

            // Act
            var response = await handler.HandleAsync(request);

            // Assert 
            Assert.NotNull(response);
            Assert.Equal(response.Email, "xyz@test.com");
            Assert.Equal(response.OrganisationName, "Test ltd.");
        }

        [Fact]
        public async void OrganisationUserExists_AndDoesNotMatchCurrentUserId_DoesNotChangeManageUserData()
        {
            var existingUserId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var organisationUserId = Guid.NewGuid();

            var manageUserData = new ManageUserData
            {
                UserId = existingUserId.ToString()
            };

            A.CallTo(() => dataAccess.GetOrganisationUser(organisationUserId))
                .Returns(manageUserData);

            A.CallTo(() => userContext.UserId)
                .Returns(currentUserId);

            var result = await GetUserDataHandler().HandleAsync(new GetUserData(organisationUserId));

            Assert.Equal(manageUserData, result);
        }

        [Fact]
        public async void OrganisationUserExists_AndMatchesCurrentUserId_ChangesManageUserDataToNotAllowRoleAndStatusChange()
        {
            var userId = Guid.NewGuid();
            var organisationUserId = Guid.NewGuid();

            var manageUserData = new ManageUserData
            {
                UserId = userId.ToString()
            };

            A.CallTo(() => dataAccess.GetOrganisationUser(organisationUserId))
                .Returns(manageUserData);

            A.CallTo(() => userContext.UserId)
                .Returns(userId);

            var result = await GetUserDataHandler().HandleAsync(new GetUserData(organisationUserId));

            Assert.False(result.CanManageRoleAndStatus);
        }

        private GetUserDataHandler GetUserDataHandler()
        {
            return new GetUserDataHandler(userContext, weeeAuthorization, dataAccess);
        }

        private IGetManageUserDataAccess CreateFakeDataAccess()
        {
            IGetManageUserDataAccess dataAccess = A.Fake<IGetManageUserDataAccess>();

            ManageUserData manageUserData = new ManageUserData
            {
                UserStatus = UserStatus.Active,
                OrganisationId = Guid.NewGuid(),
                Id = orgUserId,
                UserId = Guid.NewGuid().ToString(),
                Email = "xyz@test.com",
                FirstName = "Test",
                LastName = "Test",
                OrganisationName = "Test ltd.",
                IsCompetentAuthorityUser = false
            };

            A.CallTo(() => dataAccess.GetOrganisationUser(orgUserId)).Returns(manageUserData);

            A.CallTo(() => dataAccess.GetCompetentAuthorityUser(orgUserId)).Returns(new ManageUserData());

            return dataAccess;
        }
    }
}

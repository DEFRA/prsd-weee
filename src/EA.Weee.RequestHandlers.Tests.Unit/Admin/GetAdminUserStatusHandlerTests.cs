namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Tests.Unit.Helpers;
    using EA.Weee.Requests.Scheme;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Xunit;

    public class GetAdminUserStatusHandlerTests
    {
        /// <summary>
        /// This test ensures that an authorized user can execute requests to get user data
        /// if they provide a valid user ID.
        /// </summary>
        [Fact]
        public async void GetAdminUserStatusHandler_HappyPath_ReturnsUserStaus()
        {
            // Arrange
            Guid userId = new Guid("AC9116BC-5732-4F80-9AED-A6E2A0C4C1F1");

            IGetAdminUserDataAccess dataAccess = A.Fake<IGetAdminUserDataAccess>();
            Domain.Admin.CompetentAuthorityUser user = A.Fake<Domain.Admin.CompetentAuthorityUser>();
            A.CallTo(() => dataAccess.GetAdminUserOrDefault(userId)).Returns(user);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var userMap = A.Fake<IMap<Domain.UserStatus, UserStatus>>();
            
            A.CallTo(() => userMap.Map(user.UserStatus)).Returns(UserStatus.Active);

            GetAdminUserStatusHandler handler = new GetAdminUserStatusHandler(dataAccess, userMap, authorization);

            GetAdminUserStatus request = new GetAdminUserStatus(userId.ToString());

            // Act
            UserStatus result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(UserStatus.Active, result);
        }

        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to get scheme data.
        /// </summary>
        [Theory]
        [Trait("Authorisation", "External")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void GetAdminUserStatusHandler_WithNonExternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            Guid userId = new Guid("AC9116BC-5732-4F80-9AED-A6E2A0C4C1F1");

            IGetAdminUserDataAccess dataAccess = A.Fake<IGetAdminUserDataAccess>();
            Domain.Admin.CompetentAuthorityUser user = A.Fake<Domain.Admin.CompetentAuthorityUser>();
            A.CallTo(() => dataAccess.GetAdminUserOrDefault(userId)).Returns(user);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            var userMap = A.Fake<IMap<Domain.UserStatus, Core.Shared.UserStatus>>();
            A.CallTo(() => userMap.Map(user.UserStatus)).Returns(UserStatus.Inactive);

            GetAdminUserStatusHandler handler = new GetAdminUserStatusHandler(dataAccess, userMap, authorization);

            GetAdminUserStatus request = new GetAdminUserStatus(userId.ToString());

            // Act
            Func<Task<UserStatus>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        /// <summary>
        /// This test ensures that an ArgumentException is thrown if the scheme ID
        /// supplied cannot be found.
        /// </summary>
        [Fact]
        public async void GetAdminUserStatusHandler_WithUnknownId_ThrowsArgumentException()
        {
            // Arrange
            Guid badUserId = new Guid("88C60FAC-1172-43F2-9AA5-7E79A8877F92");

            IGetAdminUserDataAccess dataAccess = A.Fake<IGetAdminUserDataAccess>();
            A.CallTo(() => dataAccess.GetAdminUserOrDefault(badUserId)).Returns((Domain.Admin.CompetentAuthorityUser)null);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var userMap = A.Fake<IMap<Domain.UserStatus, UserStatus>>();

            GetAdminUserStatusHandler handler = new GetAdminUserStatusHandler(dataAccess, userMap, authorization);

            GetAdminUserStatus request = new GetAdminUserStatus(badUserId.ToString());

            // Act
            Func<Task<UserStatus>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}

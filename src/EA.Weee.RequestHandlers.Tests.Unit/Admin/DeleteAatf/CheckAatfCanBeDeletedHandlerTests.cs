namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DeleteAatf
{
    using EA.Weee.Core.Admin;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Admin.DeleteAatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class CheckAatfCanBeDeletedHandlerTests
    {
        private readonly IAatfDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public CheckAatfCanBeDeletedHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IAatfDataAccess>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_CheckAatfHasData_HasDataFlagIsCorrect(bool hasData)
        {
            Guid aatfId = Guid.NewGuid();

            A.CallTo(() => dataAccess.DoesAatfHaveData(aatfId)).Returns(hasData);

            CheckAatfCanBeDeleted request = new CheckAatfCanBeDeleted()
            {
                AatfId = aatfId
            };

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            var result = await handler.HandleAsync(request);

            Assert.Equal(hasData, result.HasFlag(CanAatfBeDeletedFlags.HasData));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_AatfOrganisationHasMoreAatfs_OrganisationHasMoreAatfsFlagIsCorrect(bool orgHasMoreAatfs)
        {
            Guid aatfId = Guid.NewGuid();

            A.CallTo(() => dataAccess.DoesAatfOrganisationHaveMoreAatfs(aatfId)).Returns(orgHasMoreAatfs);

            CheckAatfCanBeDeleted request = new CheckAatfCanBeDeleted()
            {
                AatfId = aatfId
            };

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            var result = await handler.HandleAsync(request);

            Assert.Equal(orgHasMoreAatfs, result.HasFlag(CanAatfBeDeletedFlags.OrganisationHasMoreAatfs));
        }

        [Fact]
        public async void HandleAsync_AatfCanBeDeleted_YesFlagIsCorrect()
        {
            Guid aatfId = Guid.NewGuid();

            A.CallTo(() => dataAccess.DoesAatfOrganisationHaveMoreAatfs(aatfId)).Returns(false);
            A.CallTo(() => dataAccess.DoesAatfHaveData(aatfId)).Returns(false);

            CheckAatfCanBeDeleted request = new CheckAatfCanBeDeleted()
            {
                AatfId = aatfId
            };

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            var result = await handler.HandleAsync(request);

            Assert.True(result.HasFlag(CanAatfBeDeletedFlags.Yes));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HandleAsync_AatfOrganisationUsers_HasActiveUsersFlagIsCorrect(bool hasActiveUsers)
        {
            Guid aatfId = Guid.NewGuid();

            A.CallTo(() => dataAccess.DoesAatfOrganisationHaveActiveUsers(aatfId)).Returns(hasActiveUsers);

            CheckAatfCanBeDeleted request = new CheckAatfCanBeDeleted()
            {
                AatfId = aatfId
            };

            CheckAatfCanBeDeletedHandler handler = new CheckAatfCanBeDeletedHandler(authorization, dataAccess);

            var result = await handler.HandleAsync(request);

            Assert.Equal(hasActiveUsers, result.HasFlag(CanAatfBeDeletedFlags.HasActiveUsers));
        }
    }
}

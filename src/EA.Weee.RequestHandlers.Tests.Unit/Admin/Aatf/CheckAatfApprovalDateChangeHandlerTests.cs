namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using FakeItEasy;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Admin.Aatf;
    using Requests.Admin.Aatf;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class CheckAatfApprovalDateChangeHandlerTests
    {
        private readonly CheckAatfApprovalDateChangeHandler handler;
        private readonly IAatfDataAccess aatfDataAccess;

        public CheckAatfApprovalDateChangeHandlerTests()
        {
            aatfDataAccess = A.Fake<IAatfDataAccess>();

            handler = new CheckAatfApprovalDateChangeHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                aatfDataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var handler = new CheckAatfApprovalDateChangeHandler(AuthorizationBuilder.CreateFromUserType(userType), aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfApprovalDateChange>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var handler = new CheckAatfApprovalDateChangeHandler(new AuthorizationBuilder().DenyRole(Roles.InternalAdmin).Build(), aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfApprovalDateChange>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }
    }
}

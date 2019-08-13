namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class CheckApprovalNumberIsUniqueHandlerTests
    {
        private readonly IFetchAatfDataAccess dataAccess;

        private readonly CheckApprovalNumberIsUniqueHandler handler;

        public CheckApprovalNumberIsUniqueHandlerTests()
        {
            dataAccess = A.Fake<IFetchAatfDataAccess>();

            handler = new CheckApprovalNumberIsUniqueHandler(AuthorizationBuilder.CreateUserWithAllRights(), dataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CheckApprovalNumberIsUniqueHandler handler = new CheckApprovalNumberIsUniqueHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckApprovalNumberIsUnique>());

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

            CheckApprovalNumberIsUniqueHandler handler = new CheckApprovalNumberIsUniqueHandler(authorization, this.dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckApprovalNumberIsUnique>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ApprovalNumberAlreadyExists_ReturnsTrue()
        {
            string approvalNumber = "app";

            Domain.AatfReturn.Aatf aatf = A.Fake<Domain.AatfReturn.Aatf>();
            A.CallTo(() => aatf.ApprovalNumber).Returns(approvalNumber);

            A.CallTo(() => dataAccess.FetchByApprovalNumber(approvalNumber)).Returns(aatf);

            bool result = await handler.HandleAsync(new CheckApprovalNumberIsUnique(approvalNumber));

            Assert.True(result);
        }

        [Fact]
        public async Task HandleAsync_ApprovalNumberDoesntExists_ReturnsFalse()
        {
            string approvalNumber = "app";

            Domain.AatfReturn.Aatf aatf = null;

            A.CallTo(() => dataAccess.FetchByApprovalNumber(approvalNumber)).Returns(aatf);

            bool result = await handler.HandleAsync(new CheckApprovalNumberIsUnique(approvalNumber));

            Assert.False(result);
        }
    }
}

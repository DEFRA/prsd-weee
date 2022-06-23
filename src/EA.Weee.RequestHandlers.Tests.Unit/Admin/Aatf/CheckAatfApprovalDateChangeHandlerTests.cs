namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using AutoFixture;
    using Core.Admin;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using Requests.Admin.Aatf;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class CheckAatfApprovalDateChangeHandlerTests
    {
        private readonly CheckAatfApprovalDateChangeHandler handler;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;

        private readonly Fixture fixture;

        public CheckAatfApprovalDateChangeHandlerTests()
        {
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            getAatfApprovalDateChangeStatus = A.Fake<IGetAatfApprovalDateChangeStatus>();
            fixture = new Fixture();

            handler = new CheckAatfApprovalDateChangeHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                aatfDataAccess, getAatfApprovalDateChangeStatus);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var handler = new CheckAatfApprovalDateChangeHandler(AuthorizationBuilder.CreateFromUserType(userType), aatfDataAccess, getAatfApprovalDateChangeStatus);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfApprovalDateChange>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var handler = new CheckAatfApprovalDateChangeHandler(new AuthorizationBuilder().DenyRole(Roles.InternalAdmin).Build(), aatfDataAccess, getAatfApprovalDateChangeStatus);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfApprovalDateChange>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_AatfDetailsShouldBeRetrieved()
        {
            var message = fixture.Create<CheckAatfApprovalDateChange>();

            var result = await handler.HandleAsync(message);

            A.CallTo(() => aatfDataAccess.GetDetails(message.AatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_ApprovalDateChangeStatusShouldBeValidatedAndReturned()
        {
            var message = fixture.Create<CheckAatfApprovalDateChange>();
            var aatf = fixture.Create<Aatf>();
            var flags = new CanApprovalDateBeChangedFlags();

            A.CallTo(() => aatfDataAccess.GetDetails(message.AatfId)).Returns(aatf);
            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(aatf, message.NewApprovalDate)).Returns(flags);

            var result = await handler.HandleAsync(message);

            result.Should().Be(flags);
        }
    }
}

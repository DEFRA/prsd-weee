namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.Aatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Aatf;
    using Xunit;

    public class GetAatfIdByComplianceYearHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess dataAccess;
        private GetAatfIdByComplianceYearHandler handler;

        public GetAatfIdByComplianceYearHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IAatfDataAccess>();

            handler = new GetAatfIdByComplianceYearHandler(authorization, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAatfIdByComplianceYearHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfIdByComplianceYear>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetAatfIdShouldBeCalled()
        {
            var result = await handler.HandleAsync(A.Dummy<GetAatfIdByComplianceYear>());

            A.CallTo(() => dataAccess.GetAatfByAatfIdAndComplianceYear(A.Dummy<Guid>(), A.Dummy<short>())).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_ComplianceYear_AatfIdMustBeReturned()
        {
            var aatf = A.Fake<Aatf>();
            var aatfId = Guid.NewGuid();
            A.CallTo(() => aatf.Id).Returns(aatfId);

            A.CallTo(() => dataAccess.GetAatfByAatfIdAndComplianceYear(A.Dummy<Guid>(), A.Dummy<short>())).Returns(aatf);

            var result = await handler.HandleAsync(A.Dummy<GetAatfIdByComplianceYear>());

            result.Should().Be(aatfId);
        }
    }
}

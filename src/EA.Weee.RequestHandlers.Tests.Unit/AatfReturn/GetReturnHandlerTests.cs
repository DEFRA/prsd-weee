namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    public class GetReturnHandlerTests
    {
        private GetReturnHandler handler;
        private readonly IGetPopulatedReturn populatedReturn;

        public GetReturnHandlerTests()
        {
            populatedReturn = A.Fake<IGetPopulatedReturn>();

            handler = new GetReturnHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                populatedReturn);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetReturnHandler(authorization,
                A.Dummy<IGetPopulatedReturn>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenReturn_GetPopulatedReturnShouldBeCalled(bool forSummary)
        {
            var returnId = Guid.NewGuid();

            var result = await handler.HandleAsync(new GetReturn(returnId, forSummary));

            A.CallTo(() => populatedReturn.GetReturnData(returnId, forSummary)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ReturnShouldBeReturned()
        {
            var returnData = new ReturnData();

            A.CallTo(() => populatedReturn.GetReturnData(A<Guid>._, A<bool>._)).Returns(returnData);

            var result = await handler.HandleAsync(A.Dummy<GetReturn>());

            result.Should().Be(returnData);
        }
    }
}

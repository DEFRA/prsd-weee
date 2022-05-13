namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class GetReportOnQuestionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private GetReportOnQuestionHandler handler;

        public GetReportOnQuestionHandlerTests()
        {
            dataAccess = A.Fake<IGenericDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new GetReportOnQuestionHandler(authorization, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetReportOnQuestionHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReportOnQuestion>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            await handler.HandleAsync(new GetReportOnQuestion());

            A.CallTo(() => dataAccess.GetAll<ReportOnQuestion>()).MustHaveHappened(1, Times.Exactly);
        }
    }
}

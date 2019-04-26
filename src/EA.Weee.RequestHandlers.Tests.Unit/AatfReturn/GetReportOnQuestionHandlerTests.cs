namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetReportOnQuestionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private GetReportOnQuestionHandler handler;

        public GetReportOnQuestionHandlerTests()
        {
            dataAccess = A.Fake<IGenericDataAccess>();

            handler = new GetReportOnQuestionHandler(dataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            await handler.HandleAsync(new GetReportOnQuestion());

            A.CallTo(() => dataAccess.GetAll<ReportOnQuestion>()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

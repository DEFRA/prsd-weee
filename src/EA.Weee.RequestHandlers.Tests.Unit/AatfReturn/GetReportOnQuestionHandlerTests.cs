namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using ReportOnQuestion = Domain.AatfReturn.ReportOnQuestion;

    public class GetReportOnQuestionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly GetReportOnQuestionHandler handler;

        public GetReportOnQuestionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGenericDataAccess>();

            handler = new GetReportOnQuestionHandler(authorization, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var handlerLocal = new GetReportOnQuestionHandler(authorization, dataAccess);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<GetReportOnQuestion>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetReportOnQuestionRequest_QuestionsShouldBeRetrieved()
        {
            await handler.HandleAsync(new GetReportOnQuestion());

            A.CallTo(() => dataAccess.GetAll<ReportOnQuestion>()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

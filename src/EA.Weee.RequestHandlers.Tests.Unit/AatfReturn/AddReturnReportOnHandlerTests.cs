namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
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
    using ReportOnQuestion = Core.AatfReturn.ReportOnQuestion;

    public class AddReturnReportOnHandlerTests
    {
        private readonly IGenericDataAccess dataAccess;
        private AddReturnReportOnHandler handler;

        public AddReturnReportOnHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGenericDataAccess>();

            handler = new AddReturnReportOnHandler(weeeAuthorization, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnReportOnHandler(authorization, A.Dummy<IGenericDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturnReportOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReportOnRequest_DataAccessAddIsCalled()
        {
            var question = A.Fake<ReportOnQuestion>();
            question.Id = 1;

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = new List<ReportOnQuestion>() { question }
            };

            await handler.HandleAsync(request);

            var returnReportOn = new ReturnReportOn(request.ReturnId, question.Id);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(new List<ReturnReportOn>() { returnReportOn })));
        }
    }
}

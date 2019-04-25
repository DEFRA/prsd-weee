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
            var questions = CreateResponse(true);

            var options = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = questions,
                Options = options
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        [Fact]
        public async Task HandleAsync_GivenAddReportOnRequest4Selected_DataAccessAddIsCalled()
        {
            var questions = CreateResponse(false);

            var options = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = questions,
                Options = options
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        private List<int> CreateResponse(bool dcf)
        {
            if (dcf)
            {
                return new List<int> { 1, 2, 3, 4, 5 };
            }
            return new List<int> { 1, 2, 3, 4 };
        }

        private List<ReturnReportOn> CreateReportedOptions(Guid returnId)
        {
            var output = new List<ReturnReportOn>();
            for (var i = 1; i <= 5; i++)
            {
                output.Add(new ReturnReportOn(returnId, i));
            }
            return output;
        }
        private List<ReportOnQuestion> CreateReportQuestions()
        {
            var output = new List<ReportOnQuestion>();
            for (var i = 1; i <= 5; i++)
            {
                output.Add(new ReportOnQuestion(i, A.Dummy<string>(), A.Dummy<string>(), null));
            }

            output[4].ParentId = 4;
            return output;
        }
    }
}

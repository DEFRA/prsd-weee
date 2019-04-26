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
            var questions = CreateSelectedOptions();

            var selectedOptions = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = questions,
                Options = selectedOptions
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId, 4);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 4))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        [Fact]
        public async Task HandleAsync_GivenAddReportOnRequestAndNonObligatedSelectedAndDcfIsYes_BothSubmitted()
        {
            var selectedOptions = CreateSelectedOptions();

            var options = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = selectedOptions,
                Options = options,
                DcfSelectedValue = "Yes"
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId, 5);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 5))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        [Fact]
        public async Task HandleAsync_GivenAddReportOnRequestAndNonObligatedNotSelectedAndDcfIsYes_NoneSubmitted()
        {
            var selectedOptions = CreateSelectedOptions();
            selectedOptions.RemoveAt(selectedOptions.Count - 1);

            var options = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = selectedOptions,
                Options = options,
                DcfSelectedValue = "Yes"
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId, 3);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 3))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        private List<int> CreateSelectedOptions()
        {
            return new List<int> { 1, 2, 3, 4 };
        }

        private List<ReturnReportOn> CreateReportedOptions(Guid returnId, int count)
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
                output.Add(new ReportOnQuestion(i, A.Dummy<string>(), A.Dummy<string>(), default(int)));
            }

            output[4].ParentId = 4;
            return output;
        }
    }
}

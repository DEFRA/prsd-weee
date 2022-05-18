namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess.DataAccess;
    using Xunit;
    using ReportOnQuestion = Core.AatfReturn.ReportOnQuestion;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    public class AddReturnReportOnHandlerTests
    {
        private readonly IGenericDataAccess dataAccess;
        private readonly WeeeContext context;
        private AddReturnReportOnHandler handler;
        private readonly Fixture fixture;

        public AddReturnReportOnHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            context = A.Fake<WeeeContext>();
            dataAccess = A.Fake<IGenericDataAccess>();
            fixture = new Fixture();

            handler = new AddReturnReportOnHandler(weeeAuthorization, dataAccess, context);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnReportOnHandler(authorization, A.Dummy<IGenericDataAccess>(), context);

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

            var returnReportOn = CreateReportedOptions(request.ReturnId);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 4))).MustHaveHappened(1, Times.Exactly);
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
                DcfSelectedValue = true
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 5))).MustHaveHappened(1, Times.Exactly);
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
                DcfSelectedValue = true
            };

            await handler.HandleAsync(request);

            var returnReportOn = CreateReportedOptions(request.ReturnId);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.Matches(r => r.Count == 3))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<IList<ReturnReportOn>>.That.IsSameAs(returnReportOn)));
        }

        [Fact]
        public async Task HandleAsync_GivenWeeeReceivedDeselected_DeleteMethodsCalled()
        {
            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = CreateSelectedOptions(),
                DeselectedOptions = new List<int>() { 1 },
                Options = CreateReportQuestions(),
                DcfSelectedValue = true
            };

            var weeeReceived = A.Fake<WeeeReceived>();
            A.CallTo(() => weeeReceived.Id).Returns(Guid.NewGuid());
            A.CallTo(() => dataAccess.GetManyByReturnId<WeeeReceived>(request.ReturnId)).Returns(new List<WeeeReceived>() { weeeReceived });

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetManyByExpression(A<WeeeReceivedAmountByWeeeReceivedIdSpecification>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeReceivedAmount>(A<IList<WeeeReceivedAmount>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeReceived>(A<IList<WeeeReceived>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<ReturnScheme>(A<IList<ReturnScheme>>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenWeeeSentOnDeselected_DeleteMethodsCalled()
        {
            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = CreateSelectedOptions(),
                DeselectedOptions = new List<int>() { 2 },
                Options = CreateReportQuestions(),
                DcfSelectedValue = true
            };

            var weeeSentOn = A.Fake<WeeeSentOn>();
            A.CallTo(() => weeeSentOn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => dataAccess.GetManyByReturnId<WeeeSentOn>(request.ReturnId)).Returns(new List<WeeeSentOn>() { weeeSentOn });

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetManyByExpression(A<WeeeSentOnAmountByWeeeSentOnIdSpecification>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<AatfAddress>(A<IList<AatfAddress>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeSentOnAmount>(A<IList<WeeeSentOnAmount>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeSentOn>(A<IList<WeeeSentOn>>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenWeeeReusedDeselected_DeleteMethodsCalled()
        {
            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = CreateSelectedOptions(),
                DeselectedOptions = new List<int>() { 3 },
                Options = CreateReportQuestions(),
                DcfSelectedValue = true
            };

            var weeeReused = A.Fake<WeeeReused>();
            A.CallTo(() => weeeReused.Id).Returns(Guid.NewGuid());
            A.CallTo(() => dataAccess.GetManyByReturnId<WeeeReused>(request.ReturnId)).Returns(new List<WeeeReused>() { weeeReused });

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetManyByExpression(A<WeeeReusedAmountByWeeeReusedIdSpecification>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.GetManyByExpression(A<WeeeReusedSiteByWeeeReusedIdSpecification>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeReusedSite>(A<IList<WeeeReusedSite>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<AatfAddress>(A<IList<AatfAddress>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeReusedAmount>(A<IList<WeeeReusedAmount>>._)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.RemoveMany<WeeeReused>(A<IList<WeeeReused>>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenNonObligatedDeselected_DeleteMethodsCalled()
        {
            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = new List<int> { 1, 2, 3, 4, 5 },
                DeselectedOptions = new List<int>() { 4 },
                Options = CreateReportQuestions(),
                DcfSelectedValue = true
            };

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.RemoveMany<NonObligatedWeee>(A<IList<NonObligatedWeee>>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenNonObligatedDcfDeselected_DeleteMethodsCalled()
        {
            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = new List<int> { 1, 2, 3, 4, 5 },
                DeselectedOptions = new List<int>() { 5 },
                Options = CreateReportQuestions(),
                DcfSelectedValue = true
            };

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.RemoveMany<NonObligatedWeee>(A<IList<NonObligatedWeee>>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReportOnRequestAndSelectedAndDcfIsNo_DcfQuestionShouldNotBeAdded()
        {
            var selectedOptions = CreateSelectedOptions();

            var options = CreateReportQuestions();

            var request = new AddReturnReportOn()
            {
                ReturnId = Guid.NewGuid(),
                SelectedOptions = selectedOptions,
                Options = options,
                DcfSelectedValue = false
            };

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.AddMany<ReturnReportOn>(A<List<ReturnReportOn>>.That.Matches(l => l.Exists(i => i.ReturnId.Equals(request.ReturnId) && i.ReportOnQuestionId.Equals((int)ReportOnQuestionEnum.NonObligatedDcf))))).MustNotHaveHappened();
        }

        private List<int> CreateSelectedOptions()
        {
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
                output.Add(fixture.Build<Core.AatfReturn.ReportOnQuestion>().With(r => r.Id, i).Create());
            }

            output[4].ParentId = 4;
            return output;
        }
    }
}

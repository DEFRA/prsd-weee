namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.CheckYourReturn;
    using RequestHandlers.Factories;
    using Requests.AatfReturn;
    using Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using Weee.Tests.Core;
    using Xunit;

    public class SubmitReturnHandlerTests
    {
        private SubmitReturnHandler handler;

        private readonly WeeeContext weeeContext;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;

        public SubmitReturnHandlerTests()
        {
            userContext = A.Fake<IUserContext>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new SubmitReturnHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                userContext,
                genericDataAccess,
                weeeContext);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new SubmitReturnHandler(authorization,
                userContext,
                genericDataAccess,
                weeeContext);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<SubmitReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            A.CallTo(() => genericDataAccess.GetById<Return>(A<Guid>._)).Returns(A.Fake<Return>());

            handler = new SubmitReturnHandler(authorization,
                userContext,
                genericDataAccess,
                weeeContext);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<SubmitReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenReturnId_ReturnShouldBeRetrieved()
        {
            var message = new SubmitReturn(Guid.NewGuid());
            var @return = GetReturn();

            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).Returns(@return);

            await handler.HandleAsync(message);

            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnCouldNotBeFound_ArgumentExceptionExpected()
        {
            A.CallTo(() => genericDataAccess.GetById<Return>(A<Guid>._)).Returns((Return)null);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<SubmitReturn>());

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_StatusShouldBeUpdated()
        {
            var message = new SubmitReturn(Guid.NewGuid());
            var @return = A.Fake<Return>();
            var userId = Guid.NewGuid();

            A.CallTo(() => @return.ReturnStatus).Returns(ReturnStatus.Created);
            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).Returns(@return);
            A.CallTo(() => userContext.UserId).Returns(userId);

            await handler.HandleAsync(message);

            A.CallTo(() => @return.UpdateSubmitted(userId.ToString())).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }

        public Return GetReturn()
        {
            return new Return(A.Fake<Operator>(), A.Fake<Quarter>(), "me");
        }
    }
}

namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using AutoFixture;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.AatfTaskList;
    using Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class SubmitReturnHandlerTests
    {
        private SubmitReturnHandler handler;

        private readonly WeeeContext weeeContext;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IFetchAatfDataAccess fetchAatfDataAccess;
        private readonly Fixture fixture;

        public SubmitReturnHandlerTests()
        {
            userContext = A.Fake<IUserContext>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            fetchAatfDataAccess = A.Fake<IFetchAatfDataAccess>();

            fixture = new Fixture();

            handler = new SubmitReturnHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                userContext,
                genericDataAccess,
                weeeContext,
                fetchAatfDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new SubmitReturnHandler(authorization,
                userContext,
                genericDataAccess,
                weeeContext,
                fetchAatfDataAccess);

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
                weeeContext,
                fetchAatfDataAccess);

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

            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnCouldNotBeFound_ArgumentExceptionExpected()
        {
            A.CallTo(() => genericDataAccess.GetById<Return>(A<Guid>._)).Returns((Return)null);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<SubmitReturn>());

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenReturn_StatusShouldBeUpdated(bool nilReturn)
        {
            var message = new SubmitReturn(Guid.NewGuid(), nilReturn);
            var @return = A.Fake<Return>();
            var userId = Guid.NewGuid();

            A.CallTo(() => @return.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Created);
            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).Returns(@return);
            A.CallTo(() => userContext.UserId).Returns(userId);

            await handler.HandleAsync(message);

            A.CallTo(() => @return.UpdateSubmitted(userId.ToString(), nilReturn)).MustHaveHappened(1, Times.Exactly)
                .Then(A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        [Fact]
        public async Task HandleAsync_GivenReturnThatIsAlreadySubmitted_StatusUpdateMustNotHaveHappened()
        {
            var message = new SubmitReturn(Guid.NewGuid());
            var @return = A.Dummy<Return>();

            var userId = Guid.NewGuid();

            A.CallTo(() => @return.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);
            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).Returns(@return);
            A.CallTo(() => userContext.UserId).Returns(userId);

            await handler.HandleAsync(message);

            A.CallTo(() => @return.UpdateSubmitted(userId.ToString(), A<bool>._)).MustNotHaveHappened();
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenReturnIsAatf_AatfsRecordsShouldBeAddedAgainstReturn()
        {
            var message = new SubmitReturn(Guid.NewGuid(), false);
            var @return = GetReturn();
            @return.ReturnStatus = ReturnStatus.Created;
            var userId = Guid.NewGuid();
            var aatfs = new List<Aatf>() { fixture.Build<Aatf>().Create() };

            A.CallTo(() => genericDataAccess.GetById<Return>(message.ReturnId)).Returns(@return);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(@return)).Returns(aatfs);

            await handler.HandleAsync(message);

            A.CallTo(() => genericDataAccess.AddMany<ReturnAatf>(A<IEnumerable<ReturnAatf>>.That.Matches(a => a.ElementAt(0).Return.Equals(@return) && a.ElementAt(0).Aatf.Equals(aatfs.ElementAt(0))))).MustHaveHappened()
                .Then(A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        public Return GetReturn()
        {
            return new Return(A.Fake<Organisation>(), A.Fake<Quarter>(), "me", FacilityType.Aatf);
        }
    }
}

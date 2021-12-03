namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using FakeItEasy.Configuration;
    using RequestHandlers.AatfReturn.NonObligated;
    using Xunit;

    public abstract class NonObligatedWeeeHandlerTestsBase<handlerT, TRequest> 
        where handlerT : IRequestHandler<TRequest, bool>
        where TRequest : IRequest<bool>
    {
        protected readonly Guid ReturnId = Guid.NewGuid();
        protected readonly Return AatfReturn = new Return(new Organisation(), new Quarter(2019, QuarterType.Q4), "someone", FacilityType.Aatf);
        protected readonly INonObligatedDataAccess NonObligatedWeeeRepoFake = A.Fake<INonObligatedDataAccess>();

        protected object mapperFake;
        protected IAssertConfiguration nonObligatedWeeeRepoCall;
        protected IAssertConfiguration mapperCall;
        protected TRequest message;

        protected abstract void ArrangeSpecifics();

        [Fact]
        public async Task Call_HandleAsync()
        {
            // Arrange
            var authorization = A.Fake<IWeeeAuthorization>();
            var returnDataAccess = A.Fake<IReturnDataAccess>();
            var getReturnById = A.CallTo(() => returnDataAccess.GetById(ReturnId));
            getReturnById.Returns(AatfReturn);
            var ensureAccessCall = A.CallTo(() => authorization.EnsureCanAccessExternalArea());
            ArrangeSpecifics();
            var handler = (handlerT)Activator.CreateInstance(typeof(handlerT), new object[] { authorization, NonObligatedWeeeRepoFake, returnDataAccess, mapperFake });

            // Act
            bool result = await handler.HandleAsync(message);

            // Assert
            Assert.True(result);
            ensureAccessCall.MustHaveHappened();
            getReturnById.MustHaveHappened();
            nonObligatedWeeeRepoCall.MustHaveHappened();
            mapperCall.MustHaveHappened();
        }
    }
}

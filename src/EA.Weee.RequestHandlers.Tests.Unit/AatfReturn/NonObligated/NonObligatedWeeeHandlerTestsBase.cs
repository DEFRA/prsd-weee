namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using FakeItEasy;
    using RequestHandlers.AatfReturn.NonObligated;
    using Xunit;

    public abstract class NonObligatedWeeeHandlerTestsBase<handlerT, TRequest> 
        where handlerT : IRequestHandler<TRequest, bool>
        where TRequest : IRequest<bool>
    {
         protected abstract IRequestHandler<TRequest, bool> CreateHandler(INonObligatedDataAccess nonObligateDataAccess, 
            IReturnDataAccess returnDataAccess, 
            IWeeeAuthorization weeeAuthorization,
            IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>> mapValue);

        protected abstract TRequest CreateMessage(Guid returnId, IList<NonObligatedValue> categoryValues);

        protected async Task ArrangeActAssert()
        {
            // Arrange
            var returnId = Guid.NewGuid();
            var mappedCollection = new List<NonObligatedWeee>();
            var aatfReturn = new Return(new Organisation(), new Quarter(2019, QuarterType.Q4), "someone", FacilityType.Aatf);
            var authorization = A.Fake<IWeeeAuthorization>();
            var nonObligatedDataAccess = A.Fake<INonObligatedDataAccess>();
            var returnDataAccess = A.Fake<IReturnDataAccess>();
            var mapper = A.Fake<IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>>>();
            var getReturnById = A.CallTo(() => returnDataAccess.GetById(returnId));
            getReturnById.Returns(aatfReturn);
            var ensureAccessCall = A.CallTo(() => authorization.EnsureCanAccessExternalArea());
            var repoCall = A.CallTo(() => nonObligatedDataAccess.AddUpdateAndClean(returnId, mappedCollection));
            var handler = CreateHandler(nonObligatedDataAccess, returnDataAccess, authorization, mapper);
            var categoryValues = new List<NonObligatedValue>();
            var message = CreateMessage(returnId, categoryValues);
            var mapperCall = A.CallTo(() => mapper.Map(categoryValues, aatfReturn));
            mapperCall.Returns(mappedCollection);

            // Act
            bool result = await handler.HandleAsync(message);

            // Assert
            Assert.True(result);
            ensureAccessCall.MustHaveHappened();
            getReturnById.MustHaveHappened();
            repoCall.MustHaveHappened();
            mapperCall.MustHaveHappened();
        }
    }
}

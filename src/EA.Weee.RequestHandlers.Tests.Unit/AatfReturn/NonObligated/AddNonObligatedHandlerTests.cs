namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using RequestHandlers.AatfReturn.NonObligated;
    using Xunit;

    public class AddNonObligatedHandlerTests : NonObligatedWeeeHandlerTestsBase<AddNonObligatedHandler, AddNonObligated>
    {
        [Fact]
        public async void HandleAsync_AddNonObligatedHandler()
        {
            await ArrangeActAssert();
        }

        protected override IRequestHandler<AddNonObligated, bool> CreateHandler(INonObligatedDataAccess nonObligateDataAccess,
            IReturnDataAccess returnDataAccess,
            IWeeeAuthorization weeeAuthorization,
            IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>> mapValue)
        {
            return new AddNonObligatedHandler(weeeAuthorization, nonObligateDataAccess, returnDataAccess, mapValue);
        }

        protected override AddNonObligated CreateMessage(Guid returnId, IList<NonObligatedValue> categoryValues)
        {
            return new AddNonObligated()
            {
                ReturnId = returnId,
                CategoryValues = categoryValues
            };
        }
    }
}

namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using Xunit;

    public class EditNonObligatedHandlerTests : NonObligatedWeeeHandlerTestsBase<EditNonObligatedHandler, EditNonObligated>
    {
        [Fact]
        public async Task HandleAsync_EditNonObligatedHandler()
        {
            await ArrangeActAssert();
        }

        protected override IRequestHandler<EditNonObligated, bool> CreateHandler(INonObligatedDataAccess nonObligateDataAccess, 
            IReturnDataAccess returnDataAccess, 
            IWeeeAuthorization weeeAuthorization,
            IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>> mapValue)
        {
            return new EditNonObligatedHandler(weeeAuthorization, nonObligateDataAccess, mapValue, returnDataAccess);
        }

        protected override EditNonObligated CreateMessage(Guid returnId, IList<NonObligatedValue> categoryValues)
        {
            return new EditNonObligated()
            {
                ReturnId = returnId,
                CategoryValues = categoryValues
            };
        }
    }
}

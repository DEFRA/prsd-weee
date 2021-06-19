namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using FakeItEasy;

    public class EditNonObligatedHandlerTests : NonObligatedWeeeHandlerTestsBase<EditNonObligatedHandler, EditNonObligated>
    {
        protected override void ArrangeSpecifics()
        {
            var editMessage = new EditNonObligated()
            {
                ReturnId = ReturnId,
            };
            this.message = editMessage;
            var mapperReturn = new Tuple<Guid, decimal?>[] { };
            var editMapperFake = A.Fake<IMapWithParameter<EditNonObligated, Return, IEnumerable<Tuple<Guid, decimal?>>>>();
            this.mapperFake = editMapperFake;
            var editMapperCall = A.CallTo(() => editMapperFake.Map(editMessage, AatfReturn));
            editMapperCall.Returns(mapperReturn);
            this.mapperCall = editMapperCall;
            this.nonObligatedWeeeRepoCall = A.CallTo(() => NonObligatedWeeeRepoFake.UpdateNonObligatedWeeeAmounts(ReturnId, mapperReturn));
        }
    }
}

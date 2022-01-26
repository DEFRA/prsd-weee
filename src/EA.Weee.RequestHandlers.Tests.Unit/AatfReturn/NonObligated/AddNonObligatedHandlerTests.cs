namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using FakeItEasy;
    using RequestHandlers.AatfReturn.NonObligated;
    using System.Collections.Generic;

    public class AddNonObligatedHandlerTests : NonObligatedWeeeHandlerTestsBase<AddNonObligatedHandler, AddNonObligated>
    {
        protected override void ArrangeSpecifics()
        {
            var addMessage = new AddNonObligated()
            {
                ReturnId = ReturnId,
            };
            this.message = addMessage;
            var mapperReturn = new NonObligatedWeee[] { };
            var editMapperFake = A.Fake<IMapWithParameter<AddNonObligated, Return, IEnumerable<NonObligatedWeee>>>();
            this.mapperFake = editMapperFake;
            var editMapperCall = A.CallTo(() => editMapperFake.Map(addMessage, AatfReturn));
            editMapperCall.Returns(mapperReturn);
            this.mapperCall = editMapperCall;
            this.nonObligatedWeeeRepoCall = A.CallTo(() => NonObligatedWeeeRepoFake.InsertNonObligatedWeee(ReturnId, mapperReturn));
        }
    }
}

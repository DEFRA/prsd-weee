namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.CheckYourReturn
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchNonObligatedWeeeForReturnHandlerTests
    {
        public List<decimal?> TonnageList;

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new FetchNonObligatedWeeeForReturnRequestHandler(A.Dummy<IFetchNonObligatedWeeeForReturnDataAccess>(), authorization);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<FetchNonObligatedWeeeForReturnRequest>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenFetchNonObligatedWeeeForReturnRequest_DataAccessFetchIsCalled(bool dcf)
        {
            Guid returnId = Guid.NewGuid();
            var request = new FetchNonObligatedWeeeForReturnRequest(returnId, dcf);
            var dataAccess = A.Fake<IFetchNonObligatedWeeeForReturnDataAccess>();

            A.CallTo(() => dataAccess.FetchNonObligatedWeeeForReturn(returnId, dcf)).Returns(TonnageList);

            var handler = new FetchNonObligatedWeeeForReturnRequestHandler(dataAccess, A.Dummy<IWeeeAuthorization>());

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.FetchNonObligatedWeeeForReturn(returnId, dcf)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

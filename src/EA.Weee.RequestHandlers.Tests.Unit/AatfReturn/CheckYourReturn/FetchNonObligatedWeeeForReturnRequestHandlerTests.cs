﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.CheckYourReturn
{
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.NonObligated;
    using RequestHandlers.Security;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchNonObligatedWeeeForReturnHandlerTests
    {
        public List<decimal?> TonnageList;

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new FetchNonObligatedWeeeForReturnRequestHandler(A.Dummy<INonObligatedDataAccess>(), authorization);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<FetchNonObligatedWeeeForReturnRequest>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            var handler = new FetchNonObligatedWeeeForReturnRequestHandler(A.Dummy<INonObligatedDataAccess>(), authorization);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<FetchNonObligatedWeeeForReturnRequest>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenFetchNonObligatedWeeeForReturnRequest_DataAccessFetchIsCalled(bool dcf)
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var request = new FetchNonObligatedWeeeForReturnRequest(returnId, organisationId, dcf);
            var dataAccess = A.Fake<INonObligatedDataAccess>();

            A.CallTo(() => dataAccess.FetchNonObligatedWeeeForReturn(returnId, dcf)).Returns(TonnageList);

            var handler = new FetchNonObligatedWeeeForReturnRequestHandler(dataAccess, A.Dummy<IWeeeAuthorization>());

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.FetchNonObligatedWeeeForReturn(returnId, dcf)).MustHaveHappened(1, Times.Exactly);
        }
    }
}

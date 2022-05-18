namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.ObligatedReused;
    using RequestHandlers.Security;
    using Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.Aatf;
    using Weee.Tests.Core;
    using Xunit;

    public class EditObligatedReusedHandlerTests
    {
        private readonly IObligatedReusedDataAccess obligatedReusedDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly EditObligatedReusedHandler handler;

        public EditObligatedReusedHandlerTests()
        {
            var authorization = A.Fake<IWeeeAuthorization>();
            obligatedReusedDataAccess = A.Dummy<IObligatedReusedDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new EditObligatedReusedHandler(authorization, obligatedReusedDataAccess, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handlerLocal = new EditObligatedReusedHandler(authorization, obligatedReusedDataAccess, genericDataAccess);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<EditObligatedReused>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAmounts_AmountsAreUpdatedCorrectly()
        {
            var updatedValues = new List<TonnageValues>()
            {
                new TonnageValues(Guid.NewGuid(), A.Dummy<int>(), 1, 2),
                new TonnageValues(Guid.NewGuid(), A.Dummy<int>(), 3, 4)
            };

            var message = new EditObligatedReused()
            {
                CategoryValues = updatedValues
            };

            var returnAmounts = new List<WeeeReusedAmount>()
            {
                new WeeeReusedAmount(A.Fake<WeeeReused>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>()),
                new WeeeReusedAmount(A.Fake<WeeeReused>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>())
            };

            A.CallTo(() => genericDataAccess.GetById<WeeeReusedAmount>(updatedValues.ElementAt(0).Id)).Returns(returnAmounts.ElementAt(0));
            A.CallTo(() => genericDataAccess.GetById<WeeeReusedAmount>(updatedValues.ElementAt(1).Id)).Returns(returnAmounts.ElementAt(1));

            await handler.HandleAsync(message);

            A.CallTo(() => obligatedReusedDataAccess.UpdateAmounts(returnAmounts.ElementAt(0), 1, 2)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => obligatedReusedDataAccess.UpdateAmounts(returnAmounts.ElementAt(1), 3, 4)).MustHaveHappened(1, Times.Exactly);
        }
    }
}

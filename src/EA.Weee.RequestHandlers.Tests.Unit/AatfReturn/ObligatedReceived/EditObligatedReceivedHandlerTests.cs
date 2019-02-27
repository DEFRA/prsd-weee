namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class EditObligatedReceivedHandlerTests
    {
        private readonly IObligatedReceivedDataAccess addObligatedReceivedDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly EditObligatedReceivedHandler handler;

        public EditObligatedReceivedHandlerTests()
        {
            var authorization = A.Fake<IWeeeAuthorization>();
            addObligatedReceivedDataAccess = A.Dummy<IObligatedReceivedDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new EditObligatedReceivedHandler(authorization, addObligatedReceivedDataAccess, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handlerLocal = new EditObligatedReceivedHandler(authorization, addObligatedReceivedDataAccess, genericDataAccess);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<EditObligatedReceived>());

            await action.Should().ThrowAsync<SecurityException>();
        }
        
        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAmounts_AmountsAreUpdatedCorrectly()
        {
            var updatedValues = new List<ObligatedValue>()
            {
                new ObligatedValue(Guid.NewGuid(), A.Dummy<int>(), 1, 2),
                new ObligatedValue(Guid.NewGuid(), A.Dummy<int>(), 3, 4)
            };

            var message = new EditObligatedReceived()
            {
                CategoryValues = updatedValues
            };

            var returnAmounts = new List<WeeeReceivedAmount>()
            {
                new WeeeReceivedAmount(A.Fake<WeeeReceived>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>()),
                new WeeeReceivedAmount(A.Fake<WeeeReceived>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>())
            };

            A.CallTo(() => genericDataAccess.GetById<WeeeReceivedAmount>(updatedValues.ElementAt(0).Id)).Returns(returnAmounts.ElementAt(0));
            A.CallTo(() => genericDataAccess.GetById<WeeeReceivedAmount>(updatedValues.ElementAt(1).Id)).Returns(returnAmounts.ElementAt(1));

            await handler.HandleAsync(message);

            A.CallTo(() => addObligatedReceivedDataAccess.UpdateAmounts(returnAmounts.ElementAt(0), 1, 2)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => addObligatedReceivedDataAccess.UpdateAmounts(returnAmounts.ElementAt(1), 3, 4)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

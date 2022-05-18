namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.Aatf;
    using Xunit;

    public class EditObligatedSentOnHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;
        private readonly EditObligatedSentOnHandler handler;

        public EditObligatedSentOnHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            obligatedSentOnDataAccess = A.Fake<IObligatedSentOnDataAccess>();

            handler = new EditObligatedSentOnHandler(authorization, obligatedSentOnDataAccess, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handlerLocal = new EditObligatedSentOnHandler(authorization, obligatedSentOnDataAccess, genericDataAccess);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<EditObligatedSentOn>());

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

            var message = new EditObligatedSentOn()
            {
                CategoryValues = updatedValues
            };

            var returnAmounts = new List<WeeeSentOnAmount>()
            {
                new WeeeSentOnAmount(A.Dummy<WeeeSentOn>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>()),
                new WeeeSentOnAmount(A.Dummy<WeeeSentOn>(), A.Dummy<int>(), A.Dummy<decimal?>(), A.Dummy<decimal?>())
            };

            A.CallTo(() => genericDataAccess.GetById<WeeeSentOnAmount>(updatedValues.ElementAt(0).Id)).Returns(returnAmounts.ElementAt(0));
            A.CallTo(() => genericDataAccess.GetById<WeeeSentOnAmount>(updatedValues.ElementAt(1).Id)).Returns(returnAmounts.ElementAt(1));

            await handler.HandleAsync(message);

            A.CallTo(() => obligatedSentOnDataAccess.UpdateAmounts(returnAmounts.ElementAt(0), 1, 2)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => obligatedSentOnDataAccess.UpdateAmounts(returnAmounts.ElementAt(1), 3, 4)).MustHaveHappened(1, Times.Exactly);
        }
    }
}

namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class EditNonObligatedHandlerTests
    {
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly EditNonObligatedHandler handler;

        public EditNonObligatedHandlerTests()
        {
            var authorization = A.Fake<IWeeeAuthorization>();
            this.nonObligatedDataAccess = A.Fake<INonObligatedDataAccess>();
            handler = new EditNonObligatedHandler(authorization, nonObligatedDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handlerLocal = new EditNonObligatedHandler(authorization, nonObligatedDataAccess);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<EditNonObligated>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAmounts_AmountsAreUpdatedCorrectly()
        {
            var updatedValues = new List<NonObligatedValue>()
            {
                new NonObligatedValue(A.Dummy<int>(), 1, false, Guid.NewGuid()),
                new NonObligatedValue(A.Dummy<int>(), 2, false, Guid.NewGuid())
            };

            var message = new EditNonObligated()
            {
                CategoryValues = updatedValues
            };

            var sig = A.CallTo(() => nonObligatedDataAccess.UpdateAmountForIds(A<IEnumerable<Tuple<Guid, decimal?>>>.Ignored));
            IEnumerable<Tuple<Guid, decimal?>> repoCallValue = null;

            sig.Invokes((p) => repoCallValue = p.Arguments.Get<IEnumerable<Tuple<Guid, decimal?>>>(0));

            await handler.HandleAsync(message);

            sig.MustHaveHappened(Repeated.Exactly.Once);
            Assert.NotNull(repoCallValue);
            Assert.Equal(updatedValues.Count(), repoCallValue.Count());
            Assert.True(updatedValues.All(n => repoCallValue.Any(t => t.Item1 == n.Id && t.Item2 == n.Tonnage)));
            Assert.True(repoCallValue.All(t => updatedValues.Any(n => t.Item1 == n.Id && t.Item2 == n.Tonnage)));
        }
    }
}

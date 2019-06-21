namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
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
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class EditNonObligatedHandlerTests
    {
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly EditNonObligatedHandler handler;

        public EditNonObligatedHandlerTests()
        {
            var authorization = A.Fake<IWeeeAuthorization>();
            this.nonObligatedDataAccess = A.Fake<INonObligatedDataAccess>();
            this.genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new EditNonObligatedHandler(authorization, nonObligatedDataAccess, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handlerLocal = new EditNonObligatedHandler(authorization, nonObligatedDataAccess, genericDataAccess);

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

            var returnAmounts = new List<NonObligatedWeee>()
            {
                new NonObligatedWeee(A.Fake<Return>(), 1, false, 1),
                new NonObligatedWeee(A.Fake<Return>(), 2, false, 2)
            };

            A.CallTo(() => genericDataAccess.GetById<NonObligatedWeee>(updatedValues.ElementAt(0).Id)).Returns(returnAmounts.ElementAt(0));
            A.CallTo(() => genericDataAccess.GetById<NonObligatedWeee>(updatedValues.ElementAt(1).Id)).Returns(returnAmounts.ElementAt(1));

            await handler.HandleAsync(message);

            A.CallTo(() => nonObligatedDataAccess.UpdateAmount(returnAmounts.ElementAt(0), 1)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => nonObligatedDataAccess.UpdateAmount(returnAmounts.ElementAt(1), 2)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

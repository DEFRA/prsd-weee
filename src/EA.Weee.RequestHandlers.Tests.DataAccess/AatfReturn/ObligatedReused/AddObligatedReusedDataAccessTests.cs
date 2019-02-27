namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class AddObligatedReusedDataAccessTests
    {
        private readonly ObligatedReusedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid aatfId;

        public AddObligatedReusedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new ObligatedReusedDataAccess(context);
            aatfId = Guid.NewGuid();
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_ValuesShouldBeAddedToContext()
        {
            var weeeReused = new WeeeReused(aatfId, Guid.NewGuid());

            decimal houseHoldTonnage = 1.000m;
            decimal nonHouseHoldTonnage = 2.000m;

            var obligatedReceivedWeee = new List<WeeeReusedAmount> { new WeeeReusedAmount(weeeReused, 1, houseHoldTonnage, nonHouseHoldTonnage), new WeeeReusedAmount(weeeReused, 2, houseHoldTonnage, nonHouseHoldTonnage) };

            var obligatedReceivedWeeeDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedAmount>());

            A.CallTo(() => context.WeeeReusedAmount).Returns(obligatedReceivedWeeeDbSet);

            dataAccess.Submit(obligatedReceivedWeee);

            context.WeeeReusedAmount.Should().AllBeEquivalentTo(obligatedReceivedWeee);
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new List<WeeeReusedAmount>());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

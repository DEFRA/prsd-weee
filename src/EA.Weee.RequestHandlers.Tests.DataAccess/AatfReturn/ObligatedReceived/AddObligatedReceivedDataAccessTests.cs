namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn.Obligated;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class AddObligatedReceivedDataAccessTests
    {
        private readonly AddObligatedReceivedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid schemeId;
        private readonly Guid aatfId;

        public AddObligatedReceivedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new AddObligatedReceivedDataAccess(context);
            schemeId = Guid.NewGuid();
            aatfId = Guid.NewGuid();
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_ValuesShouldBeAddedToContext()
        {
            var weeReceived = new WeeeReceived(schemeId, aatfId, Guid.NewGuid());

            decimal houseHoldTonnage = 1.000m;
            decimal nonHouseHoldTonnage = 2.000m;

            var obligatedReceivedWeee = new List<WeeeReceivedAmount> { new WeeeReceivedAmount(weeReceived, 1, houseHoldTonnage, nonHouseHoldTonnage), new WeeeReceivedAmount(weeReceived, 2, houseHoldTonnage, nonHouseHoldTonnage) };

            var obligatedReceivedWeeeDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReceivedAmount>());

            A.CallTo(() => context.AatfWeeReceivedAmount).Returns(obligatedReceivedWeeeDbSet);

            dataAccess.Submit(obligatedReceivedWeee);

            context.AatfWeeReceivedAmount.Should().AllBeEquivalentTo(obligatedReceivedWeee);
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new List<WeeeReceivedAmount>());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

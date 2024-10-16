﻿namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedReceived
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedGeneric;
    using RequestHandlers.AatfReturn.ObligatedReceived;
    using System;
    using System.Collections.Generic;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class AddObligatedReceivedDataAccessTests
    {
        private readonly ObligatedDataAccess<WeeeReceivedAmount> dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid schemeId;
        private readonly Guid aatfId;

        public AddObligatedReceivedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new ObligatedReceivedDataAccess(context);
            schemeId = Guid.NewGuid();
            aatfId = Guid.NewGuid();
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_ValuesShouldBeAddedToContext()
        {
            var weeReceived = new WeeeReceived(schemeId, aatfId, Guid.NewGuid());

            var houseHoldTonnage = 1.000m;
            var nonHouseHoldTonnage = 2.000m;

            var obligatedReceivedWeee = new List<WeeeReceivedAmount> { new WeeeReceivedAmount(weeReceived, 1, houseHoldTonnage, nonHouseHoldTonnage), new WeeeReceivedAmount(weeReceived, 2, houseHoldTonnage, nonHouseHoldTonnage) };

            var obligatedReceivedWeeeDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReceivedAmount>());

            A.CallTo(() => context.WeeeReceivedAmount).Returns(obligatedReceivedWeeeDbSet);

            dataAccess.Submit(obligatedReceivedWeee);

            context.WeeeReceivedAmount.Should().AllBeEquivalentTo(obligatedReceivedWeee);
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new List<WeeeReceivedAmount>());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly);
        }
    }
}

﻿namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using DataReturns;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReturnsDataTests
    {
        [Fact]
        public void ReturnsData_GivenNullReturnsList_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => new ReturnsData(null, new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ReturnsData_GivenConstructorParameters_PropertiesShouldBeMapped()
        {
            var returnsList = new List<ReturnData>();
            var returnQuarter = new Quarter(2019, QuarterType.Q1);
            List<Quarter> openQuarters = new List<Quarter>()
            {
                returnQuarter
            };

            QuarterWindow nextQuarter = new QuarterWindow(DateTime.Now, DateTime.Now.AddMonths(2), QuarterType.Q1);

            var returnsData = new ReturnsData(returnsList, returnQuarter, openQuarters, nextQuarter, DateTime.Now);

            returnsData.ReturnsList.Should().BeEquivalentTo(returnsList);
            returnsData.ReturnQuarter.Should().Be(returnQuarter);
            returnsData.OpenQuarters.Count.Should().Be(openQuarters.Count);
            returnsData.NextWindow.Should().Be(nextQuarter);
        }

        [Fact]
        public void ReturnsData_GivenNullReturnQuarter_ReturnQuarterPropertiesShouldBeNull()
        {
            var returnsData = new ReturnsData(A.Fake<List<ReturnData>>(), null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            returnsData.ReturnQuarter.Should().BeNull();
        }
    }
}

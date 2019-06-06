namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnsDataTests
    {
        [Fact]
        public void ReturnsData_GivenNullReturnsList_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => new ReturnsData(null, A.Dummy<ReturnQuarter>()));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ReturnsData_GivenConstructorParameters_PropertiesShouldBeMapped()
        {
            var returnsList = new List<ReturnData>();
            var returnQuarter = new ReturnQuarter(2019, QuarterType.Q1);

            var returnsData = new ReturnsData(returnsList, returnQuarter);

            returnsData.ReturnsList.Should().BeEquivalentTo(returnsList);
            returnsData.ReturnQuarter.Should().Be(returnQuarter);
        }

        [Fact]
        public void ReturnsData_GivenNullReturnQuarter_ReturnQuarterPropertiesShouldBeNull()
        {
            var returnQuarter = new ReturnQuarter(2019, QuarterType.Q1);

            var returnsData = new ReturnsData(A.Fake<List<ReturnData>>(), null);

            returnsData.ReturnQuarter.Should().BeNull();
        }
    }
}

namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class WeeObligatedTests
    {
        [Fact]
        public void Total_GivenB2BAndB2C_TotalShouldBeValid()
        {
            var obligated = new WeeeObligatedData(Guid.NewGuid(), A.Fake<AatfData>(), 1, 1m, 2m);

            obligated.Total.Should().Be(3);
        }

        [Fact]
        public void Total_GivenNullB2BAndNullB2C_TotalShouldBeValid()
        {
            var obligated = new WeeeObligatedData(Guid.NewGuid(), A.Fake<AatfData>(), 1, null, null);

            obligated.Total.Should().BeNull();
        }

        [Fact]
        public void Total_GivenNullB2BAndB2C_TotalShouldBeValid()
        {
            var obligated = new WeeeObligatedData(Guid.NewGuid(), A.Fake<AatfData>(), 1, null, 2m);

            obligated.Total.Should().Be(2);
        }

        [Fact]
        public void Total_GivenB2BAndNullB2C_TotalShouldBeValid()
        {
            var obligated = new WeeeObligatedData(Guid.NewGuid(), A.Fake<AatfData>(), 1, 2m, null);

            obligated.Total.Should().Be(2);
        }
    }
}

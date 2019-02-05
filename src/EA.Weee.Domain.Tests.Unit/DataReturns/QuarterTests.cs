namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class QuarterTests
    {
        [Fact]
        public void ConvertToString_ConversionIsCorrect()
        {
            var quarter = new Quarter(2019, (QuarterType)1);
            var result = quarter.ConvertToString(quarter);
            result.Should().Be("Q1 Jan - Mar");
        }
    }
}

namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System;
    using Core.Validation;
    using FluentAssertions;
    using Xunit;

    public class DecimalPlaceHelperTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void DecimalPlaces_GivenValueWithDecimalPlaces_DecimalPlacesShouldBeCorrect(int numberOfPlaces)
        {
            var value = "1";

            if (numberOfPlaces > 0)
            {
                value = $"{value}.{new string('1', numberOfPlaces)}";
            }

            decimal.Parse(value).DecimalPlaces().Should().Be(numberOfPlaces);
        }
    }
}

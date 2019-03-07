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

        [Fact]
        public void DecimalPlaces_GivenValueWithDecimalPlacesAndTrailingZeroes_DecimalPlacesShouldBeCorrect()
        {
            decimal.Parse("10.100000").DecimalPlaces().Should().Be(6);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void DecimalPlaces_GivenStringValueWithDecimalPlaces_DecimalPlacesShouldBeCorrect(int numberOfPlaces)
        {
            var value = "1";

            if (numberOfPlaces > 0)
            {
                value = $"{value}.{new string('1', numberOfPlaces)}";
            }

            value.DecimalPlaces().Should().Be(numberOfPlaces);
        }

        [Fact]
        public void DecimalPlaces_GivenStringValueWithDecimalPlacesAndTrailingZeroes_DecimalPlacesShouldBeCorrect()
        {
            "10.100000".DecimalPlaces().Should().Be(6);
        }

        [Fact]
        public void DecimalPlaces_GivenEmptyString_DecimalPlacesShouldBeZero()
        {
            string.Empty.DecimalPlaces().Should().Be(0);
        }

        [Fact]
        public void DecimalPlaces_GivenNullString_DecimalPlacesShouldBeZero()
        {
            ((string)null).DecimalPlaces().Should().Be(0);
        }

        [Fact]
        public void DecimalPlaces_GivenStringWithJustDecimal_DecimalPlacesShouldBeZero()
        {
            "1.".DecimalPlaces().Should().Be(0);
        }
    }
}

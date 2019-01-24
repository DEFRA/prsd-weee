namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Validation;
    using FluentAssertions;
    using Xunit;

    public class TonnageValueAttributeTests
    {
        private const int Category = 1;
        private readonly ValidationContext validationContext;
        private readonly TonnageValueAttribute attribute;

        public TonnageValueAttributeTests()
        {
            attribute = new TonnageValueAttribute(Category);

            var tonnageValue = new TestTonnageValue();
            validationContext = new ValidationContext(tonnageValue);
        }

        [Fact]
        public void IsValid_ValueIsNull_ReturnsTrue()
        {
            var result = attribute.IsValid(null);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("A")]
        public void IsValid_ValueIsNotANumber_ReturnsFalse(object input)
        {
            var result = attribute.IsValid(input);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("A")]
        public void ValidationResult_ValueIsNotANumber_ErrorMessageShouldBeCorrect(object input)
        {
            var result = attribute.GetValidationResult(input, validationContext);

            result.ErrorMessage.Should().Be("Category 1 tonnage value must be a numerical value");
        }

        [Fact]
        public void IsValid_ValueIsLessThanZero_ReturnsFalse()
        {
            var result = attribute.IsValid(-1);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidationResult_ValueIsLessThanZero_ErrorMessageShouldBeCorrect()
        {
            var result = attribute.GetValidationResult(-1, validationContext);

            result.ErrorMessage.Should().Be("Category 1 tonnage value must be 0 or greater");
        }

        [Fact]
        public void IsValid_ValueHasMoreThanThreeDecimalPlaces_ReturnsFalse()
        {
            var result = attribute.IsValid(1.1111M);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidationResult_ValueHasMoreThanThreeDecimalPlaces_ErrorMessageShouldBeCorrect()
        {
            var result = attribute.GetValidationResult(1.1111M, validationContext);

            result.ErrorMessage.Should().Be("Category 1 tonnage value must be 3 decimal places or less");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.11)]
        [InlineData(1.111)]
        public void IsValid_ValueIsValid_ReturnsTrue(object input)
        {
            var result = attribute.IsValid(input);

            result.Should().BeTrue();
        }

        public class TestTonnageValue
        {
            [TonnageValue(Category)]
            public string Tonnage { get; set; }
        }
    }
}

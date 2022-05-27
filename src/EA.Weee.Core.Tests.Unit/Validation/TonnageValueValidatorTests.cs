namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using FluentAssertions;
    using Xunit;

    public class TonnageValueValidatorTests
    {
        private readonly TonnageValueValidator validator;
        
        public TonnageValueValidatorTests()
        {
            validator = new TonnageValueValidator();
        }

        [Fact]
        public void Validate_ValueIsNull_ReturnsTonnageValidationResultSuccess()
        {
            var result = validator.Validate(null);

            result.Should().Be(TonnageValidationResult.Success);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("+1")]
        [InlineData("1+")]
        [InlineData("+1.1")]
        public void Validate_ValueIsNotANumber_ReturnsTonnageValidationResultNotNumerical(string input)
        {
            var result = validator.Validate(input);

            result.Type.Should().Be(TonnageValidationTypeEnum.NotNumerical);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("-2")]
        [InlineData("-1.000")]
        public void Validate_ValueIsLessThanZero_ReturnsTonnageValidationResultNotNumerical(string input)
        {
            var result = validator.Validate(input);

            result.Type.Should().Be(TonnageValidationTypeEnum.LessThanZero);
        }

        [Theory]
        [InlineData("1.1111")]
        [InlineData("10.100000")]
        public void Validate_ValueHasMoreThanThreeDecimalPlaces_ReturnsTonnageValidationResultDecimalPlaces(string value)
        {
            var decimalValue = decimal.Parse(value);
            var result = validator.Validate(decimalValue);

            result.Type.Should().Be(TonnageValidationTypeEnum.DecimalPlaces);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.11)]
        [InlineData(1.111)]
        [InlineData(" ")]
        [InlineData("  ")]
        public void Validate_ValueIsValid_ReturnsTonnageValidationResultSuccess(object input)
        {
            var result = validator.Validate(input);

            result.Should().Be(TonnageValidationResult.Success);
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("00000000000000.0")]
        [InlineData("00000000000000.00")]
        [InlineData("00000000000000.000")]
        public void Validate_GivenValueHasFourteenOrLessIntegerParts_ReturnsTonnageValidationResultSuccess(object input)
        {
            var result = validator.Validate(input);

            result.Should().Be(TonnageValidationResult.Success);
        }

        [Theory]
        [InlineData("000000000000000")]
        [InlineData("000000000000000.00")]
        [InlineData("000000000000000.000")]
        [InlineData("000000000000000.0")]
        public void Validate_GivenValueHasMoreThanFourteenIntegerParts_ReturnsTonnageValidationResultSuccess(object input)
        {
            var result = validator.Validate(input);

            result.Type.Should().Be(TonnageValidationTypeEnum.MaximumDigits);
        }
        
        [Theory]
        [InlineData("1,000,000,000.000")]
        public void Validate_GivenValueContainsCommasAndIsLessThanFourteenCharacters_ReturnsTonnageValidationResultSuccess(object input)
        {
            var result = validator.Validate(input);

            result.Should().Be(TonnageValidationResult.Success);
        }

        [Theory]
        [InlineData("1,00")]
        [InlineData("10,00")]
        [InlineData("1,000,00")]
        public void Validate_GivenCommaIsIncorrectlyUsed_ReturnsTonnageValidationResultDecimalPlaceFormat(object input)
        {
            var result = validator.Validate(input);

            result.Type.Should().Be(TonnageValidationTypeEnum.DecimalPlaceFormat);
        }

        [Fact]
        public void Validate_ValueHasMoreThanThreeDecimalPlaces_ReturnsTonnageValidationResultDecimalPlaceFormat()
        {
            var result = validator.Validate(1.1111m);

            result.Type.Should().Be(TonnageValidationTypeEnum.DecimalPlaces);
        }
    }
}

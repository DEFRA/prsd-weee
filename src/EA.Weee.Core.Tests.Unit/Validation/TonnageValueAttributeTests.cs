namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using DataReturns;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.Helpers;
    using Xunit;

    public class TonnageValueAttributeTests
    {
        private const string CategoryIdProperty = "Category";
        private const string StartOfValidationMessage = "The tonnage value";
        private readonly List<ValidationResult> validationResults;
        private const WeeeCategory Category = WeeeCategory.AutomaticDispensers;

        public TonnageValueAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void TonnageValueAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(TonnageValueAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void IsValid_GivenRelatedPropertyDoesNotExist_ValidationExceptionExpected()
        {
            var tonnageValue = new TestTonnageValueNonRelatedProperty();
            var validationContextWithInvalidModel = new ValidationContext(tonnageValue);
            var attribute = new TonnageValueAttribute(CategoryIdProperty, StartOfValidationMessage, false);

            Action action = () => attribute.Validate(1, validationContextWithInvalidModel);

            action.Should().Throw<ValidationException>().WithMessage($"Property {CategoryIdProperty} does not exist");
        }

        [Fact]
        public void IsValid_GivenRelatedPropertyIsNotWeeeCategoryValue_ValidationExceptionExpected()
        {
            var tonnageValue = new TestTonnageValueRelatedPropertyNotOfCorrectType() { Category = 15 };
            var validationContextWithInvalidModel = new ValidationContext(tonnageValue);
            var attribute = new TonnageValueAttribute(CategoryIdProperty, StartOfValidationMessage, false);

            Action action = () => attribute.Validate(1, validationContextWithInvalidModel);

            action.Should().Throw<ValidationException>().WithMessage($"Property {CategoryIdProperty} should be of type { typeof(WeeeCategory).Name }");
        }

        [Fact]
        public void IsValid_ValueIsNull_ReturnsTrue()
        {
            var result = Validate(null);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("+1")]
        [InlineData("1+")]
        [InlineData("+1.1")]
        [InlineData("-1")]
        [InlineData("-1.1")]
        public void IsValid_ValueIsNotANumber_ReturnsFalse(string input)
        {
            var result = Validate(input);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("+1")]
        [InlineData("1+")]
        [InlineData("+1.1")]
        public void ValidationResult_ValueIsNotANumber_ErrorMessageShouldBeCorrect(string input)
        {
            Validate(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be numerical");
        }

        [Theory]
        [InlineData("-0")]
        [InlineData("-00")]
        [InlineData("-1")]
        [InlineData("-1.1")]
        public void ValidationResult_ValueIsNegativeZero_ErrorMessageShouldBeCorrect(string input)
        {
            Validate(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be 0 or greater");
        }

        [Fact]
        public void IsValid_ValueIsLessThanZero_ReturnsFalse()
        {
            var result = Validate(-1);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidationResult_ValueIsLessThanZero_ErrorMessageShouldBeCorrect()
        {
            Validate(-1);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be 0 or greater");
        }

        [Theory]
        [InlineData("1.1111")]
        [InlineData("10.100000")]
        public void IsValid_ValueHasMoreThanThreeDecimalPlaces_ReturnsFalse(string value)
        {
            var decimalValue = decimal.Parse(value);
            var result = Validate(decimalValue);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("1.1111")]
        [InlineData("10.100000")]
        public void ValidationResult_ValueHasMoreThanThreeDecimalPlaces_ErrorMessageShouldBeCorrect(string value)
        {
            var decimalValue = decimal.Parse(value);
            Validate(decimalValue);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be 3 decimal places or less");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.11)]
        [InlineData(1.111)]
        [InlineData(" ")]
        [InlineData("  ")]
        public void IsValid_ValueIsValid_ReturnsTrue(object input)
        {
            var result = Validate(input);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("00000000000000.0")]
        [InlineData("00000000000000.00")]
        [InlineData("00000000000000.000")]
        public void IsValid_GivenValueHasFourteenOrLessIntegerParts_ReturnsTrue(object input)
        {
            var result = Validate(input);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("000000000000000")]
        [InlineData("000000000000000.00")]
        [InlineData("000000000000000.000")]
        [InlineData("000000000000000.0")]
        public void IsValid_GivenValueHasMoreThanFourteenIntegerParts_ReturnsFalse(object input)
        {
            var result = Validate(input);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("000000000000000")]
        [InlineData("000000000000000.00")]
        [InlineData("000000000000000.000")]
        [InlineData("000000000000000.0")]
        public void IsValid_GivenValueHasMoreThanFourteenIntegerParts_ErrorMessageShouldBeCorrect(object input)
        {
            Validate(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be numerical with 14 digits or less");
        }

        [Theory]
        [InlineData("1,000,000,000.000")]
        public void IsValid_GivenValueContainsCommasAndIsLessThanFourteenCharacters_ReturnsTrue(object input)
        {
            var result = Validate(input);

            result.Should().BeTrue();
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageIsProvidedAndErrorIsLessThanZero_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessage(-1);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} B2C must be 0 or greater");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageIsProvidedAndValueIsNotANumber_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessage("A");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} B2C must be numerical");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageIsProvidedAndValueHasMoreThanFourteenIntegerParts_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessage("0000000000000000.000");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} B2C must be numerical with 14 digits or less");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageIsProvidedAndValueHasMoreThanThreeDecimalPlaces_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessage(1.1111M);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} B2C must be 3 decimal places or less");
        }

        [Theory]
        [InlineData("1,00")]
        [InlineData("10,00")]
        [InlineData("1,000,00")]
        public void ValidationResult_GivenTypeMessageIsProvidedAndCommaIsIncorrectlyUsed_ErrorMessageShouldBeCorrect(object input)
        {
            Validate(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} must be entered in its correct format using only a comma as a thousand separator");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageAndDisplayCategoryIsProvidedAndErrorIsLessThanZero_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessageAndCategoryDisplay(-1);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} B2C must be 0 or greater");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageAndDisplayCategoryIsProvidedAndValueIsNotANumber_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessageAndCategoryDisplay("A");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} B2C must be numerical");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageAndDisplayCategoryIsProvidedAndValueHasMoreThanFourteenIntegerParts_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessageAndCategoryDisplay("0000000000000000.000");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} B2C must be numerical with 14 digits or less");
        }

        [Fact]
        public void ValidationResult_GivenTypeMessageAnDisplayCategoryIsProvidedAndValueHasMoreThanThreeDecimalPlaces_ErrorMessageShouldBeCorrect()
        {
            ValidationWithTypeMessageAndCategoryDisplay(1.1111M);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} B2C must be 3 decimal places or less");
        }

        [Theory]
        [InlineData("1,00")]
        [InlineData("10,00")]
        [InlineData("1,000,00")]
        public void ValidationResult_GivenTypeMessageAndDisplayCategoryIsProvidedAndCommaIsIncorrectlyUsed_ErrorMessageShouldBeCorrect(object input)
        {
            ValidationWithTypeMessageAndCategoryDisplay(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} B2C must be entered in its correct format using only a comma as a thousand separator");
        }

        [Fact]
        public void ValidationResult_GivenDisplayCategoryIsProvidedAndErrorIsLessThanZero_ErrorMessageShouldBeCorrect()
        {
            ValidationWithCategoryDisplay(-1);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} must be 0 or greater");
        }

        [Fact]
        public void ValidationResult_GivenDisplayCategoryIsProvidedAndValueIsNotANumber_ErrorMessageShouldBeCorrect()
        {
            ValidationWithCategoryDisplay("A");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} must be numerical");
        }

        [Fact]
        public void ValidationResult_GivenDisplayCategoryIsProvidedAndValueHasMoreThanFourteenIntegerParts_ErrorMessageShouldBeCorrect()
        {
            ValidationWithCategoryDisplay("0000000000000000.000");

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} must be numerical with 14 digits or less");
        }

        [Fact]
        public void ValidationResult_GivenDisplayCategoryIsProvidedAndValueHasMoreThanThreeDecimalPlaces_ErrorMessageShouldBeCorrect()
        {
            ValidationWithCategoryDisplay(1.1111M);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} must be 3 decimal places or less");
        }

        [Theory]
        [InlineData("1,00")]
        [InlineData("10,00")]
        [InlineData("1,000,00")]
        public void ValidationResult_GivenDisplayCategoryIsProvidedAndCommaIsIncorrectlyUsed_ErrorMessageShouldBeCorrect(object input)
        {
            ValidationWithCategoryDisplay(input);

            ValidateErrorMessage($"The tonnage value for category {(int)Category} {Category.ToDisplayString().ToLower()} must be entered in its correct format using only a comma as a thousand separator");
        }

        private void ValidationWithTypeMessage(object value)
        {
            var tonnageValueModel = new TestTonnageValueWithTypeMessage()
            {
                Category = Category,
                Tonnage = value
            };

            var validationContext = new ValidationContext(tonnageValueModel);
            Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
        }

        private void ValidationWithTypeMessageAndCategoryDisplay(object value)
        {
            var tonnageValueModel = new TestTonnageValueWithTypeMessageAndWithDisplayCategory()
            {
                Category = Category,
                Tonnage = value
            };

            var validationContext = new ValidationContext(tonnageValueModel);
            Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
        }

        private void ValidationWithCategoryDisplay(object value)
        {
            var tonnageValueModel = new TestTonnageValueWithWithDisplayCategory()
            {
                Category = Category,
                Tonnage = value
            };

            var validationContext = new ValidationContext(tonnageValueModel);
            Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
        }

        private void ValidateErrorMessage(string errorMessage)
        {
            validationResults.Count(e => e.ErrorMessage == errorMessage).Should().Be(1);
            validationResults.Count().Should().Be(1);
        }

        private bool Validate(object input)
        {
            var tonnageValueModel = TonnageValueModel(input);
            var validationContext = new ValidationContext(tonnageValueModel);

            return Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
        }

        private TestTonnageValue TonnageValueModel(object tonnage)
        {
            var tonnageValueModel = new TestTonnageValue()
            {
                Category = Category,
                Tonnage = tonnage
            };

            return tonnageValueModel;
        }

        public class TestTonnageValueNonRelatedProperty
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, false)]
            public object Tonnage { get; set; }
        }

        public class TestTonnageValueRelatedPropertyNotOfCorrectType
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, false)]
            public object Tonnage { get; set; }

            public int Category { get; set; }
        }

        public class TestTonnageValue
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, false)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }

        public class TestTonnageValueWithTypeMessage
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, "B2C", false)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }

        public class TestTonnageValueWithTypeMessageAndWithDisplayCategory
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, "B2C", true)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }

        public class TestTonnageValueWithWithDisplayCategory
        {
            [TonnageValue(CategoryIdProperty, StartOfValidationMessage, true)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }
    }
}

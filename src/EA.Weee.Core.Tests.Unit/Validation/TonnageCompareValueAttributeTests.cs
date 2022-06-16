namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class TonnageCompareValueAttributeTests
    {
        private const string Error = "Tonnage Error Message";
        private const string ErrorWithCategory = "Tonnage Error Message {0}";
        private const string AssertErrorWithCategoryString = "Tonnage Error Message 10 automatic dispensers";
        private const string CategoryIdProperty = "Category";
        private const string CompareTonnage = "CompareTonnage";
        private const WeeeCategory Category = WeeeCategory.AutomaticDispensers;
        private readonly ITonnageValueValidator tonnageTonnageValueValidator;

        private TonnageCompareValueAttribute attribute;

        public TonnageCompareValueAttributeTests()
        {
            tonnageTonnageValueValidator = A.Fake<ITonnageValueValidator>();
            A.CallTo(() => tonnageTonnageValueValidator.Validate(A<object>._)).Returns(TonnageValidationResult.Success);
        }

        [Fact]
        public void TonnageCompareValueAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(TonnageCompareValueAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>(a =>
                a.AllowMultiple.Equals(true && a.ValidOn.Equals(AttributeTargets.Property)));
        }

        [Theory]
        [InlineData("1", "A")]
        [InlineData("1", "0.0000000000000000")]
        [InlineData("1", "-1")]
        [InlineData("1", "0")]
        [InlineData("1", null)]
        [InlineData(2, "")]
        [InlineData(2, " ")]
        [InlineData(2, null)]
        public void Validate_GivenTonnageValueIsNotValid_ShouldBeValid(object tonnage,
            object compareTonnage)
        {
            var tonnageValueModel = TonnageValueModel(tonnage, compareTonnage);
            var validationContext = new ValidationContext(tonnageValueModel);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(A<object>._)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical));

            attribute = new TonnageCompareValueAttribute(CategoryIdProperty, CompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));

            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenRelatedValuePropertyDoesNotExist_ValidationMessageExpected()
        {
            var tonnageValueModel = new TestTonnageValueNonRelatedProperty()
            {
                Category = Category,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);
            attribute = new TonnageCompareValueAttribute(CategoryIdProperty, "CompareTonnageNotMatching", Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var exception = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Compare Property CompareTonnageNotMatching does not exist");
        }

        [Fact]
        public void Validate_GivenCategoryIdValuePropertyDoesNotExist_ValidationMessageExpected()
        {
            var tonnageValueModel = new TestTonnageValueNonCategoryProperty()
            {
                Category = Category,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);
            attribute = new TonnageCompareValueAttribute("NonMatchingCategory", CompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var exception = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Property NonMatchingCategory does not exist");
        }

        [Fact]
        public void Validate_GivenCategoryIdValueDoesNotExist_ValidationMessageExpected()
        {
            var tonnageValueModel = new TestTonnageValueRelatedPropertyNotOfCorrectType()
            {
                Category = 999,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);
            attribute = new TonnageCompareValueAttribute(CategoryIdProperty, CompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var exception = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Property Category should be of type WeeeCategory");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("")]
        public void Validate_GivenTonnageIsEmpty_ShouldBeValid(string tonnage)
        {
            var result = Validate(tonnage, null);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("........")]
        [InlineData("Z")]
        public void Validate_GivenTonnageIsNotANumber_ShouldBeValid(string tonnage)
        {
            var result = Validate(tonnage, null);

            result.Should().BeNull();

            A.CallTo(() => tonnageTonnageValueValidator.Validate(tonnage)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        [InlineData("1", null)]
        [InlineData(2, "")]
        [InlineData(2, " ")]
        [InlineData(2, null)]
        public void Validate_GivenTonnageIsNotEmptyAndCompareValueIsEmpty_ShouldHaveValidationMessage(object tonnage,
            object compareTonnage)
        {
            var result = Validate(tonnage, compareTonnage);

            result.ValidationResult.ErrorMessage.Should().Be(Error);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(tonnage)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        [InlineData("1", null)]
        [InlineData(2, "")]
        [InlineData(2, " ")]
        [InlineData(2, null)]
        public void Validate_GivenTonnageIsNotEmptyAndCompareValueIsEmpty_ShouldHaveValidationMessageAndErrorShouldContainCategory(object tonnage,
            object compareTonnage)
        {
            var result = ValidateWithCategoryError(tonnage, compareTonnage);

            result.ValidationResult.ErrorMessage.Should().Be(AssertErrorWithCategoryString);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(tonnage)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("0", "")]
        [InlineData("0", " ")]
        [InlineData("0", null)]
        public void Validate_GivenTonnageIsZeroAndCompareValueIsEmpty_ShouldBeValid(object tonnage,
            object compareTonnage)
        {
            var result = Validate(tonnage, compareTonnage);

            result.Should().BeNull();

            A.CallTo(() => tonnageTonnageValueValidator.Validate(tonnage)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsGreaterThanCompareTonnage_ShouldHaveValidationMessageAndErrorShouldContainCategory()
        {
            var result = ValidateWithCategoryError(2, 1);

            result.ValidationResult.ErrorMessage.Should().Be(AssertErrorWithCategoryString);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(2)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsGreaterThanCompareTonnage_ShouldHaveValidationMessage()
        {
            var result = Validate(2, 1);

            result.ValidationResult.ErrorMessage.Should().Be(Error);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(2)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsEqualThanCompareTonnage_ShouldBeValid()
        {
            var result = Validate(1, 1);

            result.Should().BeNull();

            A.CallTo(() => tonnageTonnageValueValidator.Validate(1)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsLessThanCompareTonnage_ShouldBeValid()
        {
            var result = Validate(0, 1);

            result.Should().BeNull();

            A.CallTo(() => tonnageTonnageValueValidator.Validate(0)).MustHaveHappenedOnceExactly();
        }

        private ValidationException Validate(object input, object compare)
        {
            var tonnageValueModel = TonnageValueModel(input, compare);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new TonnageCompareValueAttribute(CategoryIdProperty, CompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));

            if (result == null)
            {
                return null;
            }

            if (result is ValidationException exception)
            {
                return exception;
            }

            throw new Exception("Unknown error testing tonnage compare attribute");
        }

        private ValidationException ValidateWithCategoryError(object input, object compare)
        {
            var tonnageValueModel = TonnageValueWithCategoryErrorModel(input, compare);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new TonnageCompareValueAttribute(CategoryIdProperty, CompareTonnage, ErrorWithCategory, true)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.Tonnage, validationContext));

            if (result == null)
            {
                return null;
            }

            if (result is ValidationException exception)
            {
                return exception;
            }

            throw new Exception("Unknown error testing tonnage compare attribute");
        }

        private TestTonnageValue TonnageValueModel(object tonnage, object compare)
        {
            var tonnageValueModel = new TestTonnageValue()
            {
                Category = Category,
                Tonnage = tonnage,
                CompareTonnage = compare
            };

            return tonnageValueModel;
        }

        private TestTonnageValueWithCategoryError TonnageValueWithCategoryErrorModel(object tonnage, object compare)
        {
            var tonnageValueModel = new TestTonnageValueWithCategoryError()
            {
                Category = Category,
                Tonnage = tonnage,
                CompareTonnage = compare
            };

            return tonnageValueModel;
        }

        public class TestTonnageValueNonRelatedProperty
        {
            [TonnageCompareValue(CategoryIdProperty, "CompareTonnageNotMatching", Error)]
            public object Tonnage { get; set; }

            public object CompareTonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }

        public class TestTonnageValueNonCategoryProperty
        {
            [TonnageCompareValue("NonMatchingCategory", TonnageCompareValueAttributeTests.CompareTonnage, Error)]
            public object Tonnage { get; set; }

            public object CompareTonnage { get; set; }

            public WeeeCategory Category { get; set; }
        }

        public class TestTonnageValueRelatedPropertyNotOfCorrectType
        {
            [TonnageCompareValue(CategoryIdProperty, CompareTonnage, Error)]
            public object Tonnage { get; set; }

            public int Category { get; set; }
        }

        public class TestTonnageValue
        {
            [TonnageCompareValue(CategoryIdProperty, TonnageCompareValueAttributeTests.CompareTonnage, Error)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }

            public object CompareTonnage { get; set; }
        }

        public class TestTonnageValueWithCategoryError
        {
            [TonnageCompareValue(CategoryIdProperty, TonnageCompareValueAttributeTests.CompareTonnage, ErrorWithCategory, true)]
            public object Tonnage { get; set; }

            public WeeeCategory Category { get; set; }

            public object CompareTonnage { get; set; }
        }
    }
}

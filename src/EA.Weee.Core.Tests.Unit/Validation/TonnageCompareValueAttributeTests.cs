﻿namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using DataReturns;
    using FluentAssertions;
    using Xunit;

    public class TonnageCompareValueAttributeTests
    {
        private const string Error = "Tonnage Error Message";
        private const string ErrorWithCategory = "Tonnage Error Message {0}";
        private const string AssertErrorWithCategoryString = "Tonnage Error Message 10 automatic dispensers";
        private const string CategoryIdProperty = "Category";
        private const string CompareTonnage = "CompareTonnage";
        private readonly List<ValidationResult> validationResults;
        private const WeeeCategory Category = WeeeCategory.AutomaticDispensers;

        public TonnageCompareValueAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void TonnageCompareValueAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(TonnageCompareValueAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>(a =>
                a.AllowMultiple.Equals(true && a.ValidOn.Equals(AttributeTargets.Property)));
        }

        [Fact]
        public void Validate_GivenRelatedValuePropertyDoesNotExist_ValidationExceptionExpected()
        {
            var tonnageValueModel = new TestTonnageValueNonRelatedProperty()
            {
                Category = Category,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);

            var exception = Record.Exception(() => Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Compare Property CompareTonnageNotMatching does not exist");
        }

        [Fact]
        public void Validate_GivenCategoryIdValuePropertyDoesNotExist_ValidationExceptionExpected()
        {
            var tonnageValueModel = new TestTonnageValueNonCategoryProperty()
            {
                Category = Category,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);

            var exception = Record.Exception(() => Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Property NonMatchingCategory does not exist");
        }

        [Fact]
        public void Validate_GivenCategoryIdValueDoesNotExist_ValidationExceptionExpected()
        {
            var tonnageValueModel = new TestTonnageValueRelatedPropertyNotOfCorrectType()
            {
                Category = 999,
                Tonnage = 1
            };
            var validationContext = new ValidationContext(tonnageValueModel);

            var exception = Record.Exception(() => Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true));
            exception.Should().BeOfType<ValidationException>().Which.Message.Should().Be("Property Category should be of type WeeeCategory");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("")]
        public void Validate_GivenTonnageIsEmpty_TrueShouldBeReturned(string tonnage)
        {
            var result = Validate(tonnage, null);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("........")]
        [InlineData("Z")]
        public void Validate_GivenTonnageIsNotANumber_TrueShouldBeReturned(string tonnage)
        {
            var result = Validate(tonnage, null);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        [InlineData("1", null)]
        [InlineData(2, "")]
        [InlineData(2, " ")]
        [InlineData(2, null)]
        public void Validate_GivenTonnageIsNotEmptyAndCompareValueIsEmpty_FalseShouldBeReturned(object tonnage,
            object compareTonnage)
        {
            var result = Validate(tonnage, compareTonnage);

            result.Should().BeFalse();
            validationResults.Count.Should().Be(1);
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(Error)
            });
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        [InlineData("1", null)]
        [InlineData(2, "")]
        [InlineData(2, " ")]
        [InlineData(2, null)]
        public void Validate_GivenTonnageIsNotEmptyAndCompareValueIsEmpty_FalseShouldBeReturned_ErrorShouldContainCategory(object tonnage,
            object compareTonnage)
        {
            var result = ValidateWithCategoryError(tonnage, compareTonnage);

            result.Should().BeFalse();
            validationResults.Count.Should().Be(1);
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(AssertErrorWithCategoryString)
            });
        }

        [Theory]
        [InlineData("0", "")]
        [InlineData("0", " ")]
        [InlineData("0", null)]
        public void Validate_GivenTonnageIsZeroAndCompareValueIsEmpty_TrueShouldBeReturned(object tonnage,
            object compareTonnage)
        {
            var result = Validate(tonnage, compareTonnage);

            result.Should().BeTrue();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsGreaterThanCompareTonnage_FalseShouldBeReturned_ErrorShouldContainCategory()
        {
            var result = ValidateWithCategoryError(2, 1);

            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(AssertErrorWithCategoryString)
            });
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsGreaterThanCompareTonnage_FalseShouldBeReturned()
        {
            var result = Validate(2, 1);

            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(Error)
            });
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsEqualThanCompareTonnage_TrueShouldBeReturned()
        {
            var result = Validate(1, 1);

            result.Should().BeTrue();
        }

        [Fact]
        public void Validate_GivenTonnageTonnageIsLessThanCompareTonnage_TrueShouldBeReturned()
        {
            var result = Validate(0, 1);

            result.Should().BeTrue();
        }

        private bool Validate(object input, object compare)
        {
            var tonnageValueModel = TonnageValueModel(input, compare);
            var validationContext = new ValidationContext(tonnageValueModel);

            return Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
        }

        private bool ValidateWithCategoryError(object input, object compare)
        {
            var tonnageValueModel = TonnageValueWithCategoryErrorModel(input, compare);
            var validationContext = new ValidationContext(tonnageValueModel);

            return Validator.TryValidateObject(tonnageValueModel, validationContext, validationResults, true);
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

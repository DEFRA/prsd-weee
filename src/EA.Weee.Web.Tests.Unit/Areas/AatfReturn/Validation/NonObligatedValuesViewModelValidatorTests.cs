namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FluentAssertions;
    using FluentValidation.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class NonObligatedValuesViewModelValidatorTests
    {
        private NonObligatedValuesViewModelValidator validator;
        private readonly ICategoryValueTotalCalculator calculator;

        public NonObligatedValuesViewModelValidatorTests()
        {
            calculator = new CategoryValueTotalCalculator();
        }

        [Fact]
        public void RuleForEach_ErrorShouldOccurWhenDCFValuesIsHigherThanNonObligatedOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel(calculator);
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, model.CategoryValues.ElementAt(count).Dcf, Guid.NewGuid());
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData
            {
                NonObligatedData = nonObligatedList
            };

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                model.CategoryValues.ElementAt(count).Dcf = true;
            }

            model.Dcf = true;

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(14);
            for (var i = 0; i < validationResult.Errors.Count; i++)
            {
                var categoryId = i + 1;
                validationResult.Errors[i].PropertyName.Should().Be("CategoryValues[" + i + "].Tonnage");
                validationResult.Errors[i].ErrorMessage.Should().Be("Category " + categoryId + " tonnage must be less than or equal to " + returnData.NonObligatedData[i].Tonnage);
            }
        }

        [Fact]
        public void RuleForEach_NoErrorShouldOccurWhenDCFValuesIsLowerThanNonObligatedOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel(calculator);
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, model.CategoryValues.ElementAt(count).Dcf, Guid.NewGuid());
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData
            {
                NonObligatedData = nonObligatedList
            };

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                model.CategoryValues.ElementAt(count).Dcf = true;
            }

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleForEach_ErrorShouldOccurWhenNonObligatedValueIsLowerThanDcfValueOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel(calculator)
            {
                Dcf = true
            };
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, true, Guid.NewGuid());
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData
            {
                NonObligatedData = nonObligatedList
            };

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                model.CategoryValues.ElementAt(count).Dcf = false;
            }
            model.Dcf = false;

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(14);
            for (var i = 0; i < validationResult.Errors.Count; i++)
            {
                var categoryId = i + 1;
                validationResult.Errors[i].ErrorMessage.Should().Be("Category " + categoryId + " tonnage must be greater than or equal to " + returnData.NonObligatedData[i].Tonnage);
                validationResult.Errors[i].PropertyName.Should().Be("CategoryValues[" + i + "].Tonnage");
            }
        }

        [Fact]
        public void RuleForEach_ErrorShouldOccurWhenNonObligatedValueIsNullButDcfValueOfSameCategoryTypeHasAValue()
        {
            var model = new NonObligatedValuesViewModel(calculator)
            {
                Dcf = true
            };
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, true, Guid.NewGuid());
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData
            {
                NonObligatedData = nonObligatedList
            };

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = null;
                model.CategoryValues.ElementAt(count).Dcf = false;
            }
            model.Dcf = false;

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(14);
            for (var i = 0; i < validationResult.Errors.Count; i++)
            {
                var categoryId = i + 1;
                validationResult.Errors[i].ErrorMessage.Should().Be("Category " + categoryId + " tonnage must be greater than or equal to " + returnData.NonObligatedData[i].Tonnage);
                validationResult.Errors[i].PropertyName.Should().Be("CategoryValues[" + i + "].Tonnage");
            }
        }

        [Fact]
        public void RuleForEach_NoErrorShouldOccurWhenNonObligatedValueIsHigherThanDcfValueOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel(calculator)
            {
                Dcf = true
            };
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, true, Guid.NewGuid());
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData
            {
                NonObligatedData = nonObligatedList
            };

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                model.CategoryValues.ElementAt(count).Dcf = false;
            }
            model.Dcf = false;

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using FluentValidation.TestHelper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class NonObligatedValuesViewModelValidatorTests
    {
        private NonObligatedValuesViewModelValidator validator;

        [Fact]
        public void RuleForEach_ErrorShouldOccurWhenDCFValuesIsHigherThanNonObligatedOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel();
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, model.CategoryValues.ElementAt(count).Dcf);
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData();
            returnData.NonObligatedData = nonObligatedList;

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                model.CategoryValues.ElementAt(count).Dcf = true;
            }
            model.Dcf = true;

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void RuleForEach_NoErrorShouldOccurWhenDCFValuesIsLowerThanNonObligatedOfSameCategoryType()
        {
            var model = new NonObligatedValuesViewModel();
            List<NonObligatedData> nonObligatedList = new List<NonObligatedData>();
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 2).ToString();
                var @decimal = decimal.Parse(model.CategoryValues.ElementAt(count).Tonnage);
                var nonObligatedData = new NonObligatedData(model.CategoryValues.ElementAt(count).CategoryId, @decimal, model.CategoryValues.ElementAt(count).Dcf);
                nonObligatedList.Add(nonObligatedData);
            }
            var returnData = new ReturnData();
            returnData.NonObligatedData = nonObligatedList;

            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
                model.CategoryValues.ElementAt(count).Dcf = true;
            }

            validator = new NonObligatedValuesViewModelValidator(returnData);
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }
    }
}

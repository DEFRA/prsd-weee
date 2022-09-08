namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Xunit;

    public class RequiredTonnageAttributeTests
    {
        private readonly List<ValidationResult> validationResults;

        public RequiredTonnageAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void RequiredTonnageAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(RequiredTonnageAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void RequiredTonnageAttribute_ShouldBeDerivedFrom_RequiredAttribute()
        {
            typeof(RequiredTonnageAttribute).Should().BeDerivedFrom<RequiredTonnageBaseAttribute>();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenValuesIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            var target = ValidViewModel(SystemTime.Now, ActionEnum.Submit);
            target.CategoryValues = null;

            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => Validator.TryValidateObject(target, context, validationResults, true));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenModelIsNotValidViewModel_ArgumentNullExceptionExpected()
        {
            //arrange
            var target = new NotValidValidationTarget();
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => Validator.TryValidateObject(target, context, validationResults, true));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenNotSubmitAndNoTonnageValues_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Save);
            target.CategoryValues = new List<EvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenNotSubmitAndEmptyTonnageValues_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Save);
            target.CategoryValues = new List<EvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenNotSubmitAndTonnageValuesWithNulls_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Save);
            target.CategoryValues = new List<EvidenceCategoryValue>()
            {
                new EvidenceCategoryValue(null, null)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenNotSubmitAndTonnageValuesWithZero_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Save);
            target.CategoryValues = new List<EvidenceCategoryValue>()
            {
                new EvidenceCategoryValue("0", null)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenSubmitAndEmptyTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<EvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            ShouldBeInvalid(result);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenSubmitAndZeroTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<EvidenceCategoryValue>()
            {
                new EvidenceCategoryValue("0", null)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            ShouldBeInvalid(result);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenSubmitAndNotValidTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<EvidenceCategoryValue>()
            {
                new EvidenceCategoryValue("sdsds", null),
                new EvidenceCategoryValue("-1", null),
                new EvidenceCategoryValue("........", null)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            ShouldBeInvalid(result);

            SystemTime.Unfreeze();
        }

        private void ShouldBeInvalid(bool result)
        {
            result.Should().BeFalse();
            validationResults.Select(v => v.ErrorMessage).Should()
                .Contain("Enter a tonnage value for at least one category. The value must be 3 decimal places or less.");
        }

        private class NotValidValidationTarget
        {
            [RequiredSubmitAction]
            public DateTime? StartDate { get; set; }
        }

        private class TonnageAttributeValidationTarget : IActionModel
        {
            public ActionEnum Action { get; set; }

            [RequiredTonnage]
            public IList<EvidenceCategoryValue> CategoryValues { get; set; }

            public TonnageAttributeValidationTarget()
            {
                CategoryValues = new List<EvidenceCategoryValue>();

                foreach (var categoryValue in new EvidenceCategoryValues())
                {
                    CategoryValues.Add(categoryValue);
                }
            }
        }
        private TonnageAttributeValidationTarget ValidViewModel(DateTime date, ActionEnum action)
        {
            return new TonnageAttributeValidationTarget()
            {
                Action = action
            };
        }
    }
}
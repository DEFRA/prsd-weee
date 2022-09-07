namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using FluentAssertions;
    using Xunit;

    public class RequiredTransferTonnageAttributeTests
    {
        private readonly List<ValidationResult> validationResults;

        public RequiredTransferTonnageAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void RequiredTransferTonnageAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(RequiredTransferTonnageAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void RequiredTransferTonnageAttribute_ShouldBeDerivedFrom_RequiredAttribute()
        {
            typeof(RequiredTransferTonnageAttribute).Should().BeDerivedFrom<RequiredTonnageBaseAttribute>();
        }

        [Fact]
        public void RequiredTransferTonnageAttribute_GivenValuesIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            var target = ValidViewModel();
            target.CategoryValues = null;

            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => Validator.TryValidateObject(target, context, validationResults, true));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }
        
        [Fact]
        public void RequiredTransferTonnageAttribute_GivenEmptyTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel();
            target.CategoryValues = new List<EvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            ShouldBeInvalid(result);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTransferTonnageAttribute_GivenZeroTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel();
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
        public void RequiredTransferTonnageAttribute_GivenNotValidTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel();
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

        [Fact]
        public void RequiredTransferTonnageAttribute_GivenTonnageValue_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = ValidViewModel();
            target.CategoryValues = new List<EvidenceCategoryValue>()
            {
                new EvidenceCategoryValue("1", null),
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        private void ShouldBeInvalid(bool result)
        {
            result.Should().BeFalse();
            validationResults.Select(v => v.ErrorMessage).Should()
                .Contain("Enter a tonnage value for at least one category. The value must be 3 decimal places or less.");
        }

        private class TonnageAttributeValidationTarget
        {
            [RequiredTransferTonnage]
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
        private TonnageAttributeValidationTarget ValidViewModel()
        {
            return new TonnageAttributeValidationTarget();
        }
    }
}
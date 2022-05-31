namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Xunit;
    using WeeeCategory = Core.DataReturns.WeeeCategory;

    public class RequiredTonnageAttributeTransferEvidenceTests
    {
        private readonly List<ValidationResult> validationResults;
        private readonly Fixture fixture;
        private readonly DateTime date;
        private readonly Guid transferTonnageId;
        private readonly WeeeCategory category;

        public RequiredTonnageAttributeTransferEvidenceTests()
        {
            fixture = new Fixture();

            date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            transferTonnageId = Guid.NewGuid();

            category = fixture.Create<WeeeCategory>();

            validationResults = new List<ValidationResult>();          
        }
        
        [Fact]
        public void RequiredTonnageAttribute_GivenSubmitAndNoTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<TransferEvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();

            ShouldBeInvalid(result);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void RequiredTonnageAttribute_GivenSubmitAndEmptyTonnageValues_ValidationFailedShouldBeReturned()
        {
            //arrange
            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<TransferEvidenceCategoryValue>();

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();

            ShouldBeInvalid(result);
        }

        [Theory]
        [InlineData("120", null)]
        [InlineData("120", "")]
        [InlineData("0.00011", null)]
        [InlineData("1000", "10000")]
        public void RequiredTonnageAttribute_GivenSubmitAndValidTonnageValues_ValidationSuccessShouldBeReturned(string received, string reused)
        {
            //arrange
            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(category, transferTonnageId, 120, 120, received, reused)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("sdsds", null)]
        [InlineData("sdsds", null)]
        [InlineData("-1", null)]
        [InlineData(null, "fff")]
        [InlineData(null, "2345")]
        [InlineData("........", null)]
        [InlineData("0", null)]
        [InlineData(null, "0")]
        public void RequiredTonnageAttribute_GivenSubmitAndInvalidTonnageValues_ValidationFailedShouldBeReturned(string received, string reused)
        {
            //arrange
            var target = ValidViewModel(date, ActionEnum.Submit);
            target.CategoryValues = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(category, transferTonnageId, 120, 120, received, reused)
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            ShouldBeInvalid(result);
        }

        public void Dispose()
        {
            SystemTime.Unfreeze();
            this.Dispose();
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

        private class TransferTonnageAttributeValidationTarget : IActionModel
        {
            public ActionEnum Action { get; set; }

            [RequiredTonnage]
            public IList<TransferEvidenceCategoryValue> CategoryValues { get; set; }

            public TransferTonnageAttributeValidationTarget()
            {
                CategoryValues = new List<TransferEvidenceCategoryValue>();

                foreach (var categoryValue in new TransferEvidenceCategoryValues())
                {
                    CategoryValues.Add(categoryValue);
                }
            }
        }
        private TransferTonnageAttributeValidationTarget ValidViewModel(DateTime date, ActionEnum action)
        {
            return new TransferTonnageAttributeValidationTarget()
            {
                Action = action
            };
        }
    }
}
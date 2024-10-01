namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using EA.Weee.Web.Extensions;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Xunit;

    public class ValidateModelTests
    {

        public class TestModel
        {
            [Required(ErrorMessage = "RequiredProperty is required")]
            [StringLength(10, ErrorMessage = "RequiredProperty must not exceed 10 characters")]
            public string RequiredProperty { get; set; }

            [Range(1, 10, ErrorMessage = "RangeProperty must be between 1 and 10")]
            public int RangeProperty { get; set; }

            [StringLength(5, ErrorMessage = "StringLengthProperty must not exceed 5 characters")]
            public string StringLengthProperty { get; set; }
        }

        [Fact]
        public void ValidateModel_ValidModel_ReturnsTrue()
        {
            var model = new TestModel
            {
                RequiredProperty = "NotEmpty",
                RangeProperty = 5,
                StringLengthProperty = "Short"
            };
            var modelState = new ModelStateDictionary();

            var result = ValidationModel.ValidateModel(model, modelState);

            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
            modelState.Should().BeEmpty();
        }

        [Fact]
        public void ValidateModel_InvalidModel_ReturnsFalseAndAddsErrors()
        {
            var model = new TestModel
            {
                RequiredProperty = null,
                RangeProperty = 20,
                StringLengthProperty = "TooLong"
            };
            var modelState = new ModelStateDictionary();

            var result = ValidationModel.ValidateModel(model, modelState);

            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(3);
            modelState.Keys.Should().Contain("RequiredProperty");
            modelState.Keys.Should().Contain("RangeProperty");
            modelState.Keys.Should().Contain("StringLengthProperty");
            modelState["RequiredProperty"].Errors.Count.Should().Be(1);
            modelState["RangeProperty"].Errors.Count.Should().Be(1);
            modelState["StringLengthProperty"].Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ValidateModel_NullModel_ThrowsArgumentNullException()
        {
            var modelState = new ModelStateDictionary();

            Action act = () => ValidationModel.ValidateModel(null, modelState);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateModel_DuplicateErrors_AddsOnlyUniqueErrors()
        {
            var model = new List<TestModel>
        {
            new TestModel { RequiredProperty = null, RangeProperty = 20 },
            new TestModel { RequiredProperty = null, RangeProperty = 30 }
        };
            var modelState = new ModelStateDictionary();

            var result = ValidationModel.ValidateModel(model, modelState);

            result.Should().BeFalse("because both items in the collection have invalid properties");
            modelState.IsValid.Should().BeFalse("because there are validation errors");
            modelState.Count.Should().Be(4, "because we expect errors for RequiredProperty and RangeProperty on both items");

            modelState.Keys.Should().Contain("[0].RequiredProperty");
            modelState.Keys.Should().Contain("[0].RangeProperty");
            modelState.Keys.Should().Contain("[1].RequiredProperty");
            modelState.Keys.Should().Contain("[1].RangeProperty");

            modelState["[0].RequiredProperty"].Errors.Count.Should().Be(1);
            modelState["[0].RangeProperty"].Errors.Count.Should().Be(1);
            modelState["[1].RequiredProperty"].Errors.Count.Should().Be(1);
            modelState["[1].RangeProperty"].Errors.Count.Should().Be(1);

            modelState["[0].RequiredProperty"].Errors[0].ErrorMessage.Should().Be(modelState["[1].RequiredProperty"].Errors[0].ErrorMessage);
            modelState["[0].RangeProperty"].Errors[0].ErrorMessage.Should().Be(modelState["[1].RangeProperty"].Errors[0].ErrorMessage,
                "because both RangeProperty errors should have the same message even though the values are different");
        }

        [Fact]
        public void ValidateModel_WithPrefix_AddsErrorsWithPrefix()
        {
            var model = new TestModel
            {
                RequiredProperty = null,
                RangeProperty = 20
            };
            var modelState = new ModelStateDictionary();
            const string prefix = "TestPrefix";

            var result = ValidationModel.ValidateModel(model, modelState, prefix);

            result.Should().BeFalse();
            modelState.Count.Should().Be(2);
            modelState.Keys.Should().Contain($"{prefix}.RequiredProperty");
            modelState.Keys.Should().Contain($"{prefix}.RangeProperty");
            modelState[$"{prefix}.RequiredProperty"].Errors.Count.Should().Be(1);
            modelState[$"{prefix}.RangeProperty"].Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ValidateModel_MultipleValidationAttributes_AddsAllUniqueErrors()
        {
            var model = new TestModel { RequiredProperty = "VeryVeryLongString" };
            var modelState = new ModelStateDictionary();

            var result = ValidationModel.ValidateModel(model, modelState);

            result.Should().BeFalse();
            modelState.Keys.Should().Contain("RequiredProperty");
            modelState["RequiredProperty"].Errors.Count.Should().Be(1);
            modelState["RequiredProperty"].Errors[0].ErrorMessage.Should().Be("RequiredProperty must not exceed 10 characters");
        }

        [Fact]
        public void ValidateModel_RepeatedValidation_DoesNotAddDuplicateErrors()
        {
            var model = new TestModel { RequiredProperty = null, RangeProperty = 20 };
            var modelState = new ModelStateDictionary();

            ValidationModel.ValidateModel(model, modelState);
            var result = ValidationModel.ValidateModel(model, modelState);

            result.Should().BeFalse();
            modelState.Count.Should().Be(2);
            modelState.Keys.Should().Contain("RequiredProperty");
            modelState.Keys.Should().Contain("RangeProperty");
            modelState["RequiredProperty"].Errors.Count.Should().Be(1);
            modelState["RangeProperty"].Errors.Count.Should().Be(1);
        }
    }
}
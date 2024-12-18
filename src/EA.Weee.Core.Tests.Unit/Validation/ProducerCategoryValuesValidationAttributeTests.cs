namespace EA.Weee.Core.Tests.Unit.Validation
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class ProducerCategoryValuesValidationAttributeTests
    {
        private readonly ProducerCategoryValuesValidationAttribute attribute = new ProducerCategoryValuesValidationAttribute();

        [Fact]
        public void IsValid_NullValue_ReturnsError()
        {
            var result = attribute.GetValidationResult(null, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("Category values cannot be null");
        }

        [Fact]
        public void IsValid_EmptyList_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>();

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("Enter EEE tonnage details");
        }

        [Fact]
        public void IsValid_ZeroValues_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "0", NonHouseHold = "0" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("Enter EEE tonnage details");
        }

        [Fact]
        public void IsValid_ValidValues_ReturnsSuccess()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "1.5", NonHouseHold = "2.5" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_TotalEqualTo5_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "2.5", NonHouseHold = "2.5" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("EEE details need to total less than 5 tonnes");
        }

        [Fact]
        public void IsValid_TotalGreaterThan5_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "3", NonHouseHold = "3" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("EEE details need to total less than 5 tonnes");
        }

        [Fact]
        public void IsValid_InvalidMixedValues_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "invalid", NonHouseHold = "2.5" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("Invalid household total");
        }

        [Fact]
        public void IsValid_InvalidNonHouseholdValue_ReturnsError()
        {
            var categoryValues = new List<ProducerSubmissionCategoryValue>
            {
                new ProducerSubmissionCategoryValue { HouseHold = "1", NonHouseHold = "invalid" }
            };

            var result = attribute.GetValidationResult(categoryValues, new ValidationContext(new object()));

            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be("Invalid non-household total");
        }
    }
}
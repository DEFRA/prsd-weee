namespace EA.Weee.Core.Tests.Unit.DirectRegistrant
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using Xunit;

    public class ProducerSubmissionCategoryValueTests
    {
        [Fact]
        public void ProducerSubmissionCategoryValue_ShouldInheritFromCategoryValue()
        {
            typeof(ProducerSubmissionCategoryValue).Should().BeDerivedFrom<CategoryValue>();
        }

        [Fact]
        public void HouseHold_ShouldHaveTonnageValueAttribute()
        {
            typeof(ProducerSubmissionCategoryValue)
                .GetProperty(nameof(ProducerSubmissionCategoryValue.HouseHold))
                .Should().BeDecoratedWith<TonnageValueAttribute>(attr =>
                    attr.CategoryProperty == nameof(CategoryValue.CategoryId) &&
                    attr.StartOfValidationMessage == "The household tonnage value" &&
                    attr.DisplayCategory == true);
        }

        [Fact]
        public void NonHouseHold_ShouldHaveTonnageValueAttribute()
        {
            typeof(ProducerSubmissionCategoryValue)
                .GetProperty(nameof(ProducerSubmissionCategoryValue.NonHouseHold))
                .Should().BeDecoratedWith<TonnageValueAttribute>(attr =>
                    attr.CategoryProperty == nameof(CategoryValue.CategoryId) &&
                    attr.StartOfValidationMessage == "The non-household tonnage value" &&
                    attr.DisplayCategory == true);
        }

        [Fact]
        public void DefaultConstructor_ShouldCreateInstance()
        {
            var instance = new ProducerSubmissionCategoryValue();

            instance.Should().NotBeNull();
            instance.HouseHold.Should().BeNull();
            instance.NonHouseHold.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithHouseholdAndNonHouseholdValues_ShouldSetProperties()
        {
            var household = "100.5";
            var nonHousehold = "200.75";

            var instance = new ProducerSubmissionCategoryValue(household, nonHousehold);

            instance.HouseHold.Should().Be(household);
            instance.NonHouseHold.Should().Be(nonHousehold);
        }

        [Fact]
        public void Constructor_WithWeeeCategory_ShouldSetBaseProperties()
        {
            var category = WeeeCategory.LargeHouseholdAppliances;

            var instance = new ProducerSubmissionCategoryValue(category);

            instance.CategoryId.Should().Be((int)category);
            instance.CategoryDisplay.Should().NotBeNullOrEmpty();
            instance.HouseHold.Should().BeNull();
            instance.NonHouseHold.Should().BeNull();
        }

        [Fact]
        public void HouseHold_ShouldBeVirtual()
        {
            typeof(ProducerSubmissionCategoryValue)
                .GetProperty(nameof(ProducerSubmissionCategoryValue.HouseHold))
                .Should().BeVirtual();
        }

        [Fact]
        public void NonHouseHold_ShouldBeVirtual()
        {
            typeof(ProducerSubmissionCategoryValue)
                .GetProperty(nameof(ProducerSubmissionCategoryValue.NonHouseHold))
                .Should().BeVirtual();
        }
    }
}
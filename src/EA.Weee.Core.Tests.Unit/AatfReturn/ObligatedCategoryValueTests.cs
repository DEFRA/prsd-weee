namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.Validation;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using System;
    using Xunit;

    public class ObligatedCategoryValueTests
    {
        private readonly TonnageValueAttribute b2bAttribute;
        private readonly TonnageValueAttribute b2cAttribute;

        public ObligatedCategoryValueTests()
        {
            b2cAttribute = (TonnageValueAttribute)Attribute.GetCustomAttribute(typeof(ObligatedCategoryValue).GetProperty("B2C"), typeof(TonnageValueAttribute));
            b2bAttribute = (TonnageValueAttribute)Attribute.GetCustomAttribute(typeof(ObligatedCategoryValue).GetProperty("B2B"), typeof(TonnageValueAttribute));
        }

        [Fact]
        public void ObligatedCategoryValue_GivenHouseHoldProperty_ShouldContainTonnageValueAttribute()
        {
            b2cAttribute.Should().NotBeNull();
        }

        [Fact]
        public void ObligatedCategoryValue_GivenNonHouseHoldProperty_ShouldContainTonnageValueAttribute()
        {
            b2bAttribute.Should().NotBeNull();
        }

        [Fact]
        public void ObligatedCategoryValue_GivenHouseHoldProperty_TonnageValueAttributeShouldHaveValidPropertyName()
        {
            b2cAttribute.CategoryProperty.Should().Be("CategoryId");
        }

        [Fact]
        public void ObligatedCategoryValue_GivenNonHouseHoldProperty_TonnageValueAttributeShouldHaveValidPropertyName()
        {
            b2bAttribute.CategoryProperty.Should().Be("CategoryId");
        }

        [Fact]
        public void ObligatedCategoryValue_GivenHouseHoldProperty_TonnageValueAttributeShouldHaveValidTypeMessage()
        {
            b2cAttribute.TypeMessage.Should().Be("B2C");
        }

        [Fact]
        public void ObligatedCategoryValue_GivenNonHouseHoldProperty_TonnageValueAttributeShouldHaveValidTypeMessage()
        {
            b2bAttribute.TypeMessage.Should().Be("B2B");
        }
    }
}

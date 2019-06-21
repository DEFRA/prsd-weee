namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.Validation;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using System;
    using Xunit;

    public class NonObligatedCategoryValueTests
    {
        private readonly TonnageValueAttribute attribute;

        public NonObligatedCategoryValueTests()
        {
            attribute = (TonnageValueAttribute)Attribute.GetCustomAttribute(typeof(NonObligatedCategoryValue).GetProperty("Tonnage"), typeof(TonnageValueAttribute));
        }

        [Fact]
        public void NonObligatedCategoryValue_GivenTonnageProperty_ShouldContainTonnageValueAttribute()
        {
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void NonObligatedCategoryValue_GivenTonnageProperty_TonnageValueAttributeShouldHaveValidPropertyName()
        {
            attribute.CategoryProperty.Should().Be("CategoryId");
        }
    }
}

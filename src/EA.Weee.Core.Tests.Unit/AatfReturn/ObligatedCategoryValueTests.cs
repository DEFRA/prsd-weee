namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.Validation;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using System;
    using Xunit;

    public class ObligatedCategoryValueTests
    {
        [Fact]
        public void ObligatedCategoryValue_ShouldHaveSerializableAttribute()
        {
            typeof(ObligatedCategoryValue).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void ObligatedCategoryValue_B2CProperty_ShouldHaveTonnageAttribute()
        {
            typeof(ObligatedCategoryValue).GetProperty("B2C").Should().BeDecoratedWith<TonnageValueAttribute>(
                a => a.CategoryProperty.Equals("CategoryId") 
                     && a.StartOfValidationMessage.Equals("The tonnage value")
                     && a.TypeMessage.Equals("B2C"));
        }

        [Fact]
        public void ObligatedCategoryValue_B2BProperty_ShouldHaveTonnageAttribute()
        {
            typeof(ObligatedCategoryValue).GetProperty("B2B").Should().BeDecoratedWith<TonnageValueAttribute>(
                a => a.CategoryProperty.Equals("CategoryId")
                     && a.StartOfValidationMessage.Equals("The tonnage value")
                     && a.TypeMessage.Equals("B2B"));
        }
    }
}

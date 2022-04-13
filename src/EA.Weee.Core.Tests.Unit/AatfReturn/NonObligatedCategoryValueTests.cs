namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.Validation;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using System;
    using Xunit;

    public class NonObligatedCategoryValueTests
    {
        [Fact]
        public void NonObligatedCategoryValue_ShouldHaveSerializableAttribute()
        {
            typeof(NonObligatedCategoryValue).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void NonObligatedCategoryValue_TonnageProperty_ShouldHaveTonnageAttribute()
        {
            typeof(NonObligatedCategoryValue).GetProperty("Tonnage").Should().BeDecoratedWith<TonnageValueAttribute>(
                a => a.CategoryProperty.Equals("CategoryId") 
                     && a.StartOfValidationMessage.Equals("The tonnage value") 
                     && a.DisplayCategory.Equals(false));
        }
    }
}

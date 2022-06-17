namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Validation;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
    using Xunit;

    public class EvidenceCategoryValueTests
    {
        [Fact]
        public void EvidenceCategoryValue_ShouldHaveSerializableAttribute()
        {
            typeof(EvidenceCategoryValue).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void EvidenceCategoryValue_Constructor_ShouldSetProperties()
        {
            //arrange
            var received = Faker.Lorem.GetFirstWord();
            var reused = Faker.Lorem.Sentence();

            //act
            var category = new EvidenceCategoryValue(received, reused);

            //assert
            category.Received.Should().Be(received);
            category.Reused.Should().Be(reused);
        }

        [Fact]
        public void EvidenceCategoryValue_ShouldInheritFrom_CategoryValue()
        {
            typeof(EvidenceCategoryValue).Should().BeDerivedFrom<CategoryValue>();
        }

        [Fact]
        public void EvidenceCategoryValue_ReceivedProperty_ShouldBeDecoratedWith_TonnageValueAttribute()
        {
            typeof(EvidenceCategoryValue).GetProperty("Received")
                .Should()
                .BeDecoratedWith<TonnageValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") && t.StartOfValidationMessage.Equals("The tonnage value") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void EvidenceCategoryValue_ReusedProperty_ShouldBeDecoratedWith_TonnageValueAttribute()
        {
            typeof(EvidenceCategoryValue).GetProperty("Reused")
                .Should()
                .BeDecoratedWith<TonnageValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") && t.StartOfValidationMessage.Equals("The tonnage value") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void EvidenceCategoryValue_ReusedProperty_ShouldBeDecoratedWith_TonnageCompareValueAttribute()
        {
            typeof(EvidenceCategoryValue).GetProperty("Reused")
                .Should()
                .BeDecoratedWith<TonnageCompareValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") &&
                    t.ComparePropertyName.Equals("Received") &&
                    t.ErrorMessage.Equals("The reused tonnage for category {0} must be equivalent or lower than the received tonnage"));
        }
    }
}

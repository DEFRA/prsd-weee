namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Shared;
    using Core.Validation;
    using DataReturns;
    using FluentAssertions;
    using Xunit;

    public class TransferEvidenceCategoryValueTests
    {
        [Fact]
        public void TransferEvidenceCategoryValue_ShouldHaveSerializableAttribute()
        {
            typeof(TransferEvidenceCategoryValue).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void TransferEvidenceCategoryValue_Constructor_ShouldSetProperties()
        {
            //arrange
            var fixture = new Fixture();

            var availableReceived = fixture.Create<decimal?>();
            var availableReused = fixture.Create<decimal?>();
            var category = fixture.Create<WeeeCategory>();
            var transferTonnageId = fixture.Create<Guid>();
            var received = fixture.Create<string>();
            var reused = fixture.Create<string>();

            //act
            var result = new TransferEvidenceCategoryValue(category, transferTonnageId, availableReceived, availableReused,
                received,
                reused);

            //assert
            result.TransferTonnageId.Should().Be(transferTonnageId);
            result.CategoryId.Should().Be(category.ToInt());
            result.AvailableReceived.Should().Be(availableReceived);
            result.AvailableReused.Should().Be(availableReused);
            result.Reused.Should().Be(reused);
            result.Received.Should().Be(received);
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ShouldInheritFrom_CategoryValue()
        {
            typeof(TransferEvidenceCategoryValue).Should().BeDerivedFrom<CategoryValue>();
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ReceivedProperty_ShouldBeDecoratedWith_TonnageValueAttribute()
        {
            typeof(TransferEvidenceCategoryValue).GetProperty("Received")
                .Should()
                .BeDecoratedWith<TonnageValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") && t.StartOfValidationMessage.Equals("The transfer received in tonnes") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ReusedProperty_ShouldBeDecoratedWith_TonnageValueAttribute()
        {
            typeof(TransferEvidenceCategoryValue).GetProperty("Reused")
                .Should()
                .BeDecoratedWith<TonnageValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") && t.StartOfValidationMessage.Equals("The transfer reused in tonnes") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ReusedPropertyToCompareToReceived_ShouldBeDecoratedWith_TonnageCompareValueAttribute()
        {
            typeof(TransferEvidenceCategoryValue).GetProperty("Reused")
                .Should()
                .BeDecoratedWith<TonnageCompareValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") &&
                    t.ComparePropertyName.Equals("Received") &&
                    t.ErrorMessage.Equals("The transfer reused in tonnes for category {0} must be equivalent or lower than the transfer received") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ReusedPropertyToCompareToAvailableReused_ShouldBeDecoratedWith_TonnageCompareValueAttribute()
        {
            typeof(TransferEvidenceCategoryValue).GetProperty("Reused")
                .Should()
                .BeDecoratedWith<TonnageCompareValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") &&
                    t.ComparePropertyName.Equals("AvailableReused") &&
                    t.ErrorMessage.Equals("The transfer reused in tonnes for category {0} must be equivalent or lower than the total reused available") &&
                    t.DisplayCategory.Equals(true));
        }

        [Fact]
        public void TransferEvidenceCategoryValue_ReceivedPropertyToCompareToAvailableReceived_ShouldBeDecoratedWith_TonnageCompareValueAttribute()
        {
            typeof(TransferEvidenceCategoryValue).GetProperty("Received")
                .Should()
                .BeDecoratedWith<TonnageCompareValueAttribute>(t =>
                    t.CategoryProperty.Equals("CategoryId") &&
                    t.ComparePropertyName.Equals("AvailableReceived") &&
                    t.ErrorMessage.Equals("The transfer received in tonnes for category {0} must be equivalent or lower than the total received available") &&
                    t.DisplayCategory.Equals(true));
        }
    }
}

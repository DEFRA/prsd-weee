namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
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
    }
}

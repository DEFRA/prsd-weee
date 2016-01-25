namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class WeeeCollectedReturnVersionTests
    {
        [Fact]
        public void ConstructsWeeeCollectedReturnVersion_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeCollectedReturnVersion(null));
        }

        [Fact]
        public void ConstructsWeeeCollectedReturnVersion_AddsDataReturnVersion_ToDataReturnVersionsList()
        {
            // Arrange
            var dataReturnVersion = new DataReturnVersion(A.Dummy<DataReturn>());

            // Act
            var collectedReturnVersion = new WeeeCollectedReturnVersion(dataReturnVersion);

            // Assert
            Assert.Contains(dataReturnVersion, collectedReturnVersion.DataReturnVersions);
        }

        [Fact]
        public void AddWeeeCollectedAmount_AddsToWeeeCollectedAmounts()
        {
            // Arrange
            var collectedReturnVersion = new WeeeCollectedReturnVersion(A.Dummy<DataReturnVersion>());
            var amount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._);

            // Act
            collectedReturnVersion.AddWeeeCollectedAmount(amount);

            // Assert
            Assert.Contains(amount, collectedReturnVersion.WeeeCollectedAmounts);
        }
    }
}

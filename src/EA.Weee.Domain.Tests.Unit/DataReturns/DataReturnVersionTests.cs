namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Producer;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class DataReturnVersionTests
    {
        [Fact]
        public void ConstructsDataReturnVersion_WithNullDataReturn_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataReturnVersion(null));
        }

        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var eeeOutputAmount = new EeeOutputAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<RegisteredProducer>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddEeeOutputAmount(eeeOutputAmount);

            // Assert
            Assert.Contains(eeeOutputAmount, returnVersion.EeeOutputAmounts);
        }

        [Fact]
        public void AddWeeeCollectedAmount_AddsToWeeeCollectedAmounts()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var weeeCollectedAmount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);

            // Assert
            Assert.Contains(weeeCollectedAmount, returnVersion.WeeeCollectedAmounts);
        }

        [Fact]
        public void AddAatfDeliveryLocation_AddsToAatfDeliveryLocations()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var deliveredAmount = new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AatfDeliveryLocation>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAatfDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, returnVersion.AatfDeliveredAmounts);
        }

        [Fact]
        public void AddAeDeliveryLocation_AddsToAeDeliveryLocations()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var deliveredAmount = new AeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AeDeliveryLocation>(), A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, returnVersion.AeDeliveredAmounts);
        }
    }
}

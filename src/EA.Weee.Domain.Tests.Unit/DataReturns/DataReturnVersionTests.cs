namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
            var eeeOutputAmount = new EeeOutputAmount(A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<RegisteredProducer>(), A.Fake<DataReturnVersion>());

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
            var weeeCollectedAmount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>());

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
            var aatfDeliveryLocation = new AatfDeliveryLocation("Test", "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAatfDeliveryLocation(aatfDeliveryLocation);

            // Assert
            Assert.Contains(aatfDeliveryLocation, returnVersion.AatfDeliveryLocations);
        }

        [Fact]
        public void AddAeDeliveryLocation_AddsToAeDeliveryLocations()
        {
            // Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());
            var deliveryLocation = new AeDeliveryLocation("Test", "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>());

            // Act
            returnVersion.AddAeDeliveryLocation(deliveryLocation);

            // Assert
            Assert.Contains(deliveryLocation, returnVersion.AeDeliveryLocations);
        }
    }
}

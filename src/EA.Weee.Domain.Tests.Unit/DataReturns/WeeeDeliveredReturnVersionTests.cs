namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class WeeeDeliveredReturnVersionTests
    {
        [Fact]
        public void ConstructsWeeeDeliveredReturnVersion_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeDeliveredReturnVersion(null));
        }

        [Fact]
        public void ConstructsWeeeDeliveredReturnVersion_AddsDataReturnVersion_ToDataReturnVersionsList()
        {
            // Arrange
            var dataReturnVersion = new DataReturnVersion(A.Dummy<DataReturn>());

            // Act
            var collectedReturnVersion = new WeeeDeliveredReturnVersion(dataReturnVersion);

            // Assert
            Assert.Contains(dataReturnVersion, collectedReturnVersion.DataReturnVersions);
        }

        [Fact]
        public void AddAatfDeliveryLocation_AddsToAatfDeliveryLocations()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(A.Fake<DataReturnVersion>());
            var deliveredAmount = new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AatfDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddAatfDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.AatfDeliveredAmounts);
        }

        [Fact]
        public void AddAeDeliveryLocation_AddsToAeDeliveryLocations()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(A.Fake<DataReturnVersion>());
            var deliveredAmount = new AeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AeDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddAeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.AeDeliveredAmounts);
        }
    }
}

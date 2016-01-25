namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Obligation;
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
        public void AddWeeeDeliveredAmount_WithNullWeeeDeliveredAmount_ThrowsArgumentNullException()
        {
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(A.Fake<DataReturnVersion>());

            Assert.Throws<ArgumentNullException>(() => weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(null));
        }

        [Fact]
        public void AddWeeeDeliveredAmount_WithAatfDeliveryLocation_AddsToWeeeDeliveredAmounts()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(A.Fake<DataReturnVersion>());
            var deliveredAmount = new WeeeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AatfDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.WeeeDeliveredAmounts);
        }

        [Fact]
        public void AddWeeeDeliveredAmount_WithAeDeliveryLocation_AddsToWeeeDeliveredAmounts()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(A.Fake<DataReturnVersion>());
            var deliveredAmount = new WeeeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AeDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.WeeeDeliveredAmounts);
        }
    }
}

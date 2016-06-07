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
        public void AddWeeeDeliveredAmount_WithNullWeeeDeliveredAmount_ThrowsArgumentNullException()
        {
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();

            Assert.Throws<ArgumentNullException>(() => weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(null));
        }

        [Fact]
        public void AddWeeeDeliveredAmount_WithAatfDeliveryLocation_AddsToWeeeDeliveredAmounts()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            var deliveredAmount = new WeeeDeliveredAmount(
                ObligationType.B2B, A.Dummy<WeeeCategory>(), A.Dummy<decimal>(), A.Fake<AatfDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.WeeeDeliveredAmounts);
        }

        [Fact]
        public void AddWeeeDeliveredAmount_WithAeDeliveryLocation_AddsToWeeeDeliveredAmounts()
        {
            // Arrange
            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            var deliveredAmount = new WeeeDeliveredAmount(
                ObligationType.B2B, A.Dummy<WeeeCategory>(), A.Dummy<decimal>(), A.Fake<AeDeliveryLocation>());

            // Act
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredAmount);

            // Assert
            Assert.Contains(deliveredAmount, weeeDeliveredReturnVersion.WeeeDeliveredAmounts);
        }
    }
}

namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class WeeeCollectedReturnVersionTests
    {
        [Fact]
        public void AddWeeeCollectedAmount_AddsToWeeeCollectedAmounts()
        {
            // Arrange
            var collectedReturnVersion = new WeeeCollectedReturnVersion();
            var amount = new WeeeCollectedAmount(A<WeeeCollectedAmountSourceType>._, ObligationType.B2B, A<WeeeCategory>._, A<decimal>._);

            // Act
            collectedReturnVersion.AddWeeeCollectedAmount(amount);

            // Assert
            Assert.Contains(amount, collectedReturnVersion.WeeeCollectedAmounts);
        }
    }
}

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
        public void AddWeeeCollectedAmount_AddsToWeeeCollectedAmounts()
        {
            // Arrange
            var collectedReturnVersion = new WeeeCollectedReturnVersion();
            var amount = new WeeeCollectedAmount(
                A.Dummy<WeeeCollectedAmountSourceType>(), ObligationType.B2B, A.Dummy<WeeeCategory>(), A.Dummy<decimal>());

            // Act
            collectedReturnVersion.AddWeeeCollectedAmount(amount);

            // Assert
            Assert.Contains(amount, collectedReturnVersion.WeeeCollectedAmounts);
        }
    }
}

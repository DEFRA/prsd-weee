namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class AeDeliveredAmountTests
    {
        [Fact]
        public void ConstructsAeDeliveredAmount_WithNullAeDeliveryLocation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}

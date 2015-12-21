namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class WeeeDeliveredAmountTests
    {
        [Fact]
        public void ConstructsWeeeDeliveredAmount_WithNullAatfDeliveryLocation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, (AatfDeliveryLocation)null));
        }

        [Fact]
        public void ConstructsWeeeDeliveredAmount_WithNullAeDeliveryLocation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeeeDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, (AeDeliveryLocation)null));
        }
    }
}

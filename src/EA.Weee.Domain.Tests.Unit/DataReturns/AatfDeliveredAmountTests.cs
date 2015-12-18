namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class AatfDeliveredAmountTests
    {
        [Fact]
        public void ConstructsAatfDeliveredAmount_WithNullAatfDeliveryLocation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}

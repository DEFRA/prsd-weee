namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Xunit;

    public class AeDeliveryLocationTests
    {
        [Fact]
        public void ConstructsAeDeliveryLocation_WithNullApprovalNumber_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveryLocation(null, "Test"));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithEmptyApprovalNumber_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AeDeliveryLocation(string.Empty, "Test"));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithNullOperatorName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveryLocation("Test", null));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithEmptyOperatorName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AeDeliveryLocation("Test", string.Empty));
        }
    }
}

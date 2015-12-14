namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class AatfDeliveryLocationTests
    {
        [Fact]
        public void ConstructsAatfDeliveryLocation_WithNullAatfApprovalNumber_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveryLocation(null, "Test"));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithEmptyAatfApprovalNumber_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AatfDeliveryLocation(string.Empty, "Test"));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithNullFacilityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveryLocation("Test", null));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithEmptyFacilityName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AatfDeliveryLocation("Test", string.Empty));
        }
    }
}

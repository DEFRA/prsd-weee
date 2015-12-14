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
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveryLocation(null, "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithEmptyAatfApprovalNumber_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AatfDeliveryLocation(string.Empty, "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithNullFacilityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveryLocation("Test", null, A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithEmptyFacilityName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AatfDeliveryLocation("Test", string.Empty, A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAatfDeliveryLocation_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveryLocation("Test", "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}

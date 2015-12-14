namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class AeDeliveryLocationTests
    {
        [Fact]
        public void ConstructsAeDeliveryLocation_WithNullApprovalNumber_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveryLocation(null, "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithEmptyApprovalNumber_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AeDeliveryLocation(string.Empty, "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithNullOperatorName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveryLocation("Test", null, A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithEmptyOperatorName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AeDeliveryLocation("Test", string.Empty, A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAeDeliveryLocation_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AeDeliveryLocation("Test", "Test", A<ObligationType>._, A<WeeeCategory>._, A<decimal>._, null));
        }
    }
}

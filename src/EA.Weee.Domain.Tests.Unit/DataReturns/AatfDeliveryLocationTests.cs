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

        [Fact]
        public void AatfDeliveryLocation_EqualsNullParameter_ReturnsFalse()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.NewAatfDeliveryLocation;

            Assert.NotEqual(aatfDeliveryLocation, null);
        }

        [Fact]
        public void AatfDeliveryLocation_EqualsObjectParameter_ReturnsFalse()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.NewAatfDeliveryLocation;

            Assert.NotEqual(aatfDeliveryLocation, new object());
        }

        [Fact]
        public void AatfDeliveryLocation_EqualsSameInstance_ReturnsTrue()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.NewAatfDeliveryLocation;

            Assert.Equal(aatfDeliveryLocation, aatfDeliveryLocation);
        }

        [Fact]
        public void AatfDeliveryLocation_EqualsaatfDeliveryLocationSameDetails_ReturnsTrue()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.NewAatfDeliveryLocation;
            var aatfDeliveryLocation2 = AatfDeliveryLocationBuilder.NewAatfDeliveryLocation;

            Assert.Equal(aatfDeliveryLocation, aatfDeliveryLocation2);
        }

        [Fact]
        public void AatfDeliveryLocation_EqualsAatfDeliveryLocationDifferentApprovalNumber_ReturnsFalse()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.WithApprovalNumber("AAA");
            var aatfDeliveryLocation2 = AatfDeliveryLocationBuilder.WithApprovalNumber("BBB");

            Assert.NotEqual(aatfDeliveryLocation, aatfDeliveryLocation2);
        }

        [Fact]
        public void AatfDeliveryLocation_EqualsAatfDeliveryLocationDifferentFacilityName_ReturnsFalse()
        {
            var aatfDeliveryLocation = AatfDeliveryLocationBuilder.WithFacilityName("AAA");
            var aatfDeliveryLocation2 = AatfDeliveryLocationBuilder.WithFacilityName("BBB");

            Assert.NotEqual(aatfDeliveryLocation, aatfDeliveryLocation2);
        }

        private class AatfDeliveryLocationBuilder
        {
            private string approvalNumber;
            private string facilityName;

            public AatfDeliveryLocationBuilder()
            {
                approvalNumber = "WEE/AA233/ATF";
                facilityName = "Test name";
            }

            public AatfDeliveryLocation Build()
            {
                return new AatfDeliveryLocation(approvalNumber, facilityName);
            }

            public static AatfDeliveryLocation NewAatfDeliveryLocation
            {
                get { return new AatfDeliveryLocationBuilder().Build(); }
            }

            public static AatfDeliveryLocation WithApprovalNumber(string approvalNumber)
            {
                var builder = new AatfDeliveryLocationBuilder();
                builder.approvalNumber = approvalNumber;

                return builder.Build();
            }

            public static AatfDeliveryLocation WithFacilityName(string facilityName)
            {
                var builder = new AatfDeliveryLocationBuilder();
                builder.facilityName = facilityName;

                return builder.Build();
            }
        }
    }
}

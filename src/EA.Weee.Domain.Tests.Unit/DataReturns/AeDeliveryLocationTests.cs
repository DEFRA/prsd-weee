namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Domain.DataReturns;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
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
        public void AeDeliveryLocation_EqualsNullParameter_ReturnsFalse()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.NewAeDeliveryLocation;

            Assert.NotEqual(aeDeliveryLocation, null);
        }

        [Fact]
        public void AeDeliveryLocation_EqualsObjectParameter_ReturnsFalse()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.NewAeDeliveryLocation;

            Assert.NotEqual(aeDeliveryLocation, new object());
        }

        [Fact]
        public void AeDeliveryLocation_EqualsSameInstance_ReturnsTrue()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.NewAeDeliveryLocation;

            Assert.Equal(aeDeliveryLocation, aeDeliveryLocation);
        }

        [Fact]
        public void AeDeliveryLocation_EqualsAeDeliveryLocationSameDetails_ReturnsTrue()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.NewAeDeliveryLocation;
            var aeDeliveryLocation2 = AeDeliveryLocationBuilder.NewAeDeliveryLocation;

            Assert.Equal(aeDeliveryLocation, aeDeliveryLocation2);
        }

        [Fact]
        public void AeDeliveryLocation_EqualsAeDeliveryLocationDifferentApprovalNumber_ReturnsFalse()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.WithApprovalNumber("AAA");
            var aeDeliveryLocation2 = AeDeliveryLocationBuilder.WithApprovalNumber("BBB");

            Assert.NotEqual(aeDeliveryLocation, aeDeliveryLocation2);
        }

        [Fact]
        public void AeDeliveryLocation_EqualsAeDeliveryLocationDifferentFacilityName_ReturnsFalse()
        {
            var aeDeliveryLocation = AeDeliveryLocationBuilder.WithOperatorName("AAA");
            var aeDeliveryLocation2 = AeDeliveryLocationBuilder.WithOperatorName("BBB");

            Assert.NotEqual(aeDeliveryLocation, aeDeliveryLocation2);
        }

        private class AeDeliveryLocationBuilder
        {
            private string approvalNumber;
            private string operatorName;

            public AeDeliveryLocationBuilder()
            {
                approvalNumber = "WEE/AA233/ATF";
                operatorName = "Test name";
            }

            public AeDeliveryLocation Build()
            {
                return new AeDeliveryLocation(approvalNumber, operatorName);
            }

            public static AeDeliveryLocation NewAeDeliveryLocation
            {
                get { return new AeDeliveryLocationBuilder().Build(); }
            }

            public static AeDeliveryLocation WithApprovalNumber(string approvalNumber)
            {
                var builder = new AeDeliveryLocationBuilder();
                builder.approvalNumber = approvalNumber;

                return builder.Build();
            }

            public static AeDeliveryLocation WithOperatorName(string operatorName)
            {
                var builder = new AeDeliveryLocationBuilder();
                builder.operatorName = operatorName;

                return builder.Build();
            }
        }
    }
}

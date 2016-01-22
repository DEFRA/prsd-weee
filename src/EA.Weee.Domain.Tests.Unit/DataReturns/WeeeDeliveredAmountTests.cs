namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
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

        [Fact]
        public void WeeeDeliveredAmount_EqualsNullParameter_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.NewWeeeDeliveredAmount;

            Assert.NotEqual(weeeDeliveredAmount, null);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsObjectParameter_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.NewWeeeDeliveredAmount;

            Assert.NotEqual(weeeDeliveredAmount, new object());
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsSameInstance_ReturnsTrue()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.NewWeeeDeliveredAmount;

            Assert.Equal(weeeDeliveredAmount, weeeDeliveredAmount);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountSameDetails_ReturnsTrue()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.NewWeeeDeliveredAmount;
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.NewWeeeDeliveredAmount;

            Assert.Equal(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountDifferentObligationType_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithObligationType(ObligationType.B2B);
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithObligationType(ObligationType.B2C);

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountDifferentWeeeCategory_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithWeeeCategory(WeeeCategory.AutomaticDispensers);
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithWeeeCategory(WeeeCategory.ConsumerEquipment);

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountDifferentTonnage_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithTonnage(5);
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithTonnage(10);

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountDifferentAatfDeliveryLocation_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithAatfDeliveryLocation(new AatfDeliveryLocation("AAA", "Test"));
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithAatfDeliveryLocation(new AatfDeliveryLocation("BBB", "Test"));

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_EqualsWeeeDeliveredAmountDifferentAeDeliveryLocation_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithAeDeliveryLocation(new AeDeliveryLocation("AAA", "Test"));
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithAeDeliveryLocation(new AeDeliveryLocation("BBB", "Test"));

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        [Fact]
        public void WeeeDeliveredAmount_WithAatfDeliveryLocation_EqualsWeeeDeliveredAmount_WithAeDeliveryLocation_ReturnsFalse()
        {
            var weeeDeliveredAmount = WeeeDeliveredAmountBuilder.WithAatfDeliveryLocation(new AatfDeliveryLocation("AAA", "Test"));
            var weeeDeliveredAmount2 = WeeeDeliveredAmountBuilder.WithAeDeliveryLocation(new AeDeliveryLocation("AAA", "Test"));

            Assert.NotEqual(weeeDeliveredAmount, weeeDeliveredAmount2);
        }

        private class WeeeDeliveredAmountBuilder
        {
            private ObligationType obligationType;
            private decimal tonnage;
            private WeeeCategory weeeCategory;
            private AatfDeliveryLocation aatfDeliveryLocation;
            private AeDeliveryLocation aeDeliveryLocation;

            public WeeeDeliveredAmountBuilder()
            {
                obligationType = ObligationType.B2C;
                tonnage = 100;
                weeeCategory = WeeeCategory.AutomaticDispensers;
                aatfDeliveryLocation = new AatfDeliveryLocation("ApprovalNumber", "FacilityName");
            }

            public WeeeDeliveredAmount Build()
            {
                return aeDeliveryLocation == null ?
                    new WeeeDeliveredAmount(obligationType, weeeCategory, tonnage, aatfDeliveryLocation) :
                    new WeeeDeliveredAmount(obligationType, weeeCategory, tonnage, aeDeliveryLocation);
            }

            public static WeeeDeliveredAmount NewWeeeDeliveredAmount
            {
                get { return new WeeeDeliveredAmountBuilder().Build(); }
            }

            public static WeeeDeliveredAmount WithObligationType(ObligationType obligationType)
            {
                var builder = new WeeeDeliveredAmountBuilder();
                builder.obligationType = obligationType;

                return builder.Build();
            }

            public static WeeeDeliveredAmount WithWeeeCategory(WeeeCategory category)
            {
                var builder = new WeeeDeliveredAmountBuilder();
                builder.weeeCategory = category;

                return builder.Build();
            }

            public static WeeeDeliveredAmount WithTonnage(decimal tonnage)
            {
                var builder = new WeeeDeliveredAmountBuilder();
                builder.tonnage = tonnage;

                return builder.Build();
            }

            public static WeeeDeliveredAmount WithAatfDeliveryLocation(AatfDeliveryLocation aatfDeliveryLocation)
            {
                var builder = new WeeeDeliveredAmountBuilder();
                builder.aatfDeliveryLocation = aatfDeliveryLocation;

                return builder.Build();
            }

            public static WeeeDeliveredAmount WithAeDeliveryLocation(AeDeliveryLocation aeDeliveryLocation)
            {
                var builder = new WeeeDeliveredAmountBuilder();
                builder.aeDeliveryLocation = aeDeliveryLocation;

                return builder.Build();
            }
        }
    }
}

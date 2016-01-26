namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class WeeeCollectedAmountTests
    {
        [Fact]
        public void WeeeCollectedAmount_EqualsNullParameter_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.NewWeeeCollectedAmount;

            Assert.NotEqual(weeeCollectedAmount, null);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsObjectParameter_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.NewWeeeCollectedAmount;

            Assert.NotEqual(weeeCollectedAmount, new object());
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsSameInstance_ReturnsTrue()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.NewWeeeCollectedAmount;

            Assert.Equal(weeeCollectedAmount, weeeCollectedAmount);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsWeeeCollectedAmountSameDetails_ReturnsTrue()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.NewWeeeCollectedAmount;
            var weeeCollectedAmount2 = WeeeCollectedAmountBuilder.NewWeeeCollectedAmount;

            Assert.Equal(weeeCollectedAmount, weeeCollectedAmount2);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsWeeeCollectedAmountDifferentObligationType_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.WithObligationType(ObligationType.B2B);
            var weeeCollectedAmount2 = WeeeCollectedAmountBuilder.WithObligationType(ObligationType.B2C);

            Assert.NotEqual(weeeCollectedAmount, weeeCollectedAmount2);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsWeeeCollectedAmountDifferentWeeeCategory_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.WithWeeeCategory(WeeeCategory.AutomaticDispensers);
            var weeeCollectedAmount2 = WeeeCollectedAmountBuilder.WithWeeeCategory(WeeeCategory.ConsumerEquipment);

            Assert.NotEqual(weeeCollectedAmount, weeeCollectedAmount2);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsWeeeCollectedAmountDifferentTonnage_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.WithTonnage(5);
            var weeeCollectedAmount2 = WeeeCollectedAmountBuilder.WithTonnage(10);

            Assert.NotEqual(weeeCollectedAmount, weeeCollectedAmount2);
        }

        [Fact]
        public void WeeeCollectedAmount_EqualsWeeeCollectedAmountDifferentSourceType_ReturnsFalse()
        {
            var weeeCollectedAmount = WeeeCollectedAmountBuilder.WithSourceType(WeeeCollectedAmountSourceType.Dcf);
            var weeeCollectedAmount2 = WeeeCollectedAmountBuilder.WithSourceType(WeeeCollectedAmountSourceType.Distributor);

            Assert.NotEqual(weeeCollectedAmount, weeeCollectedAmount2);
        }

        private class WeeeCollectedAmountBuilder
        {
            private ObligationType obligationType;
            private decimal tonnage;
            private WeeeCategory weeeCategory;
            private WeeeCollectedAmountSourceType sourceType;

            public WeeeCollectedAmountBuilder()
            {
                obligationType = ObligationType.B2C;
                tonnage = 100;
                weeeCategory = WeeeCategory.AutomaticDispensers;
                sourceType = WeeeCollectedAmountSourceType.Dcf;
            }

            public WeeeCollectedAmount Build()
            {
                return new WeeeCollectedAmount(sourceType, obligationType, weeeCategory, tonnage);
            }

            public static WeeeCollectedAmount NewWeeeCollectedAmount
            {
                get { return new WeeeCollectedAmountBuilder().Build(); }
            }

            public static WeeeCollectedAmount WithObligationType(ObligationType obligationType)
            {
                var builder = new WeeeCollectedAmountBuilder();
                builder.obligationType = obligationType;

                return builder.Build();
            }

            public static WeeeCollectedAmount WithWeeeCategory(WeeeCategory category)
            {
                var builder = new WeeeCollectedAmountBuilder();
                builder.weeeCategory = category;

                return builder.Build();
            }

            public static WeeeCollectedAmount WithTonnage(decimal tonnage)
            {
                var builder = new WeeeCollectedAmountBuilder();
                builder.tonnage = tonnage;

                return builder.Build();
            }

            public static WeeeCollectedAmount WithSourceType(WeeeCollectedAmountSourceType sourceType)
            {
                var builder = new WeeeCollectedAmountBuilder();
                builder.sourceType = sourceType;

                return builder.Build();
            }
        }
    }
}

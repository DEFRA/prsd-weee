namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Producer;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using Xunit;

    public class EeeOutputAmountTests
    {
        [Fact]
        public void ConstructsEeeOutputAmount_WithNullRegisteredProducer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EeeOutputAmount(ObligationType.B2B, A.Dummy<WeeeCategory>(),
                A.Dummy<decimal>(), null));
        }

        [Fact]
        public void EeeOutputAmount_EqualsNullParameter_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.NewEeeOutputAmount;

            Assert.NotEqual(eeeOutputAmount, null);
        }

        [Fact]
        public void EeeOutputAmount_EqualsObjectParameter_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.NewEeeOutputAmount;

            Assert.NotEqual(eeeOutputAmount, new object());
        }

        [Fact]
        public void EeeOutputAmount_EqualsSameInstance_ReturnsTrue()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.NewEeeOutputAmount;

            Assert.Equal(eeeOutputAmount, eeeOutputAmount);
        }

        [Fact]
        public void EeeOutputAmount_EqualsEeeOutputAmountSameDetails_ReturnsTrue()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.NewEeeOutputAmount;
            var eeeOutputAmount2 = EeeOutputAmountBuilder.NewEeeOutputAmount;

            Assert.Equal(eeeOutputAmount, eeeOutputAmount2);
        }

        [Fact]
        public void EeeOutputAmount_EqualsEeeOutputAmountDifferentObligationType_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.WithObligationType(ObligationType.B2B);
            var eeeOutputAmount2 = EeeOutputAmountBuilder.WithObligationType(ObligationType.B2C);

            Assert.NotEqual(eeeOutputAmount, eeeOutputAmount2);
        }

        [Fact]
        public void EeeOutputAmount_EqualsEeeOutputAmountDifferentWeeeCategory_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.WithWeeeCategory(WeeeCategory.AutomaticDispensers);
            var eeeOutputAmount2 = EeeOutputAmountBuilder.WithWeeeCategory(WeeeCategory.ConsumerEquipment);

            Assert.NotEqual(eeeOutputAmount, eeeOutputAmount2);
        }

        [Fact]
        public void EeeOutputAmount_EqualsEeeOutputAmountDifferentTonnage_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.WithTonnage(5);
            var eeeOutputAmount2 = EeeOutputAmountBuilder.WithTonnage(10);

            Assert.NotEqual(eeeOutputAmount, eeeOutputAmount2);
        }

        [Fact]
        public void EeeOutputAmount_EqualsEeeOutputAmountDifferentRegistered_ReturnsFalse()
        {
            var eeeOutputAmount = EeeOutputAmountBuilder.WithUnequalRegisteredProducer();
            var eeeOutputAmount2 = EeeOutputAmountBuilder.NewEeeOutputAmount;

            Assert.NotEqual(eeeOutputAmount, eeeOutputAmount2);
        }

        private class EeeOutputAmountBuilder
        {
            private ObligationType obligationType;
            private decimal tonnage;
            private WeeeCategory weeeCategory;
            private bool registeredProducerEquality;

            public EeeOutputAmountBuilder()
            {
                obligationType = ObligationType.B2C;
                tonnage = 100;
                weeeCategory = WeeeCategory.AutomaticDispensers;
                registeredProducerEquality = true;
            }

            public EeeOutputAmount Build()
            {
                var registeredProducer = A.Fake<RegisteredProducer>();
                A.CallTo(() => registeredProducer.Equals(A<RegisteredProducer>._))
                    .Returns(registeredProducerEquality);

                return new EeeOutputAmount(obligationType, weeeCategory, tonnage, registeredProducer);
            }

            public static EeeOutputAmount NewEeeOutputAmount
            {
                get { return new EeeOutputAmountBuilder().Build(); }
            }

            public static EeeOutputAmount WithObligationType(ObligationType obligationType)
            {
                var builder = new EeeOutputAmountBuilder();
                builder.obligationType = obligationType;

                return builder.Build();
            }

            public static EeeOutputAmount WithWeeeCategory(WeeeCategory category)
            {
                var builder = new EeeOutputAmountBuilder();
                builder.weeeCategory = category;

                return builder.Build();
            }

            public static EeeOutputAmount WithTonnage(decimal tonnage)
            {
                var builder = new EeeOutputAmountBuilder();
                builder.tonnage = tonnage;

                return builder.Build();
            }

            public static EeeOutputAmount WithUnequalRegisteredProducer()
            {
                var builder = new EeeOutputAmountBuilder();
                builder.registeredProducerEquality = false;

                return builder.Build();
            }
        }
    }
}

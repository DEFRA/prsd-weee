namespace EA.Weee.Domain.Tests.Unit
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Lookup;
    using Obligation;
    using Xunit;

    public class ExtensionMethodsTests
    {
        [Fact]
        public void EqualElements_NullCollections_ReturnsTrue()
        {
            List<int> a = null;
            List<int> b = null;

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_OneNullCollection_ReturnsFalse()
        {
            List<int> a = new List<int>();
            List<int> b = null;

            Assert.False(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameInstances_ReturnsTrue()
        {
            List<int> a = new List<int>();

            Assert.True(a.ElementsEqual(a));
        }

        [Fact]
        public void EqualElements_DifferentCount_ReturnsFalse()
        {
            List<int> a = new List<int> { 1 };
            List<int> b = new List<int> { 1, 2 };

            Assert.False(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameElementsSameOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 5 };

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameElementsDifferentOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 2, 1, 5, 3, 4 };
            List<int> b = new List<int> { 5, 3, 1, 4, 2 };

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_DifferentElements_ReturnsFalse()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 10 };

            Assert.False(a.ElementsEqual(b));
        }

        [Fact]
        public void UnorderedEqual_NullCollections_ReturnsTrue()
        {
            List<int> a = null;
            List<int> b = null;

            Assert.True(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_OneNullCollection_ReturnsFalse()
        {
            List<int> a = new List<int>();
            List<int> b = null;

            Assert.False(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_SameInstances_ReturnsTrue()
        {
            List<int> a = new List<int>();

            Assert.True(a.UnorderedEqual(a));
        }

        [Fact]
        public void UnorderedEqual_DifferentCount_ReturnsFalse()
        {
            List<int> a = new List<int> { 1 };
            List<int> b = new List<int> { 1, 2 };

            Assert.False(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_SameElementsSameOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 5 };

            Assert.True(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_SameElementsDifferentOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 2, 1, 5, 3, 4 };
            List<int> b = new List<int> { 5, 3, 1, 4, 2 };

            Assert.True(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_DifferentElements_ReturnsFalse()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 10 };

            Assert.False(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_ElementsDotNotImplementIComparable_DifferentCount_ReturnsFalse()
        {
            var a = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10)
            };

            var b = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10)
            };

            Assert.False(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_ElementsDotNotImplementIComparable_SameElementsSameOrder_ReturnsTrue()
        {
            var a = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
            };

            var b = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
            };

            Assert.True(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_ElementsDotNotImplementIComparable_SameElementsDifferentOrder_ReturnsTrue()
        {
            var a = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
            };

            var b = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10)
            };

            Assert.True(a.UnorderedEqual(b));
        }

        [Fact]
        public void UnorderedEqual_ElementsDotNotImplementIComparable_DifferentElements_ReturnsFalse()
        {
            var a = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
            };

            var b = new List<WeeeCollectedAmount>
            {
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.AutomaticDispensers, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2B, WeeeCategory.DisplayEquipment, 10),
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2B, WeeeCategory.ConsumerEquipment, 10),
            };

            Assert.False(a.UnorderedEqual(b));
        }

        //same, different order, duplicate items - repeat for non-comparable items
    }
}

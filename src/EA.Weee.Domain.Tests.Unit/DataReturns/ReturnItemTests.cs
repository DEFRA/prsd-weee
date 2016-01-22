namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class ReturnItemTests
    {
        [Fact]
        public void ConstructsReturnItem_WithTonnageLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReturnItem(ObligationType.B2B, A<WeeeCategory>._, -1));
        }

        [Theory]
        [InlineData(ObligationType.None)]
        [InlineData(ObligationType.Both)]
        public void ConstructsReturnItem_WithObligationTypeNotB2BOrB2C_ThrowsInvalidOperationException(ObligationType obligationType)
        {
            Assert.Throws<InvalidOperationException>(() => new ReturnItem(obligationType, A<WeeeCategory>._, A<decimal>._));
        }

        [Fact]
        public void ReturnItem_EqualsNullParameter_ReturnsFalse()
        {
            var returnItem = ReturnItemBuilder.NewReturnItem;

            Assert.NotEqual(returnItem, null);
        }

        [Fact]
        public void ReturnItem_EqualsObjectParameter_ReturnsFalse()
        {
            var returnItem = ReturnItemBuilder.NewReturnItem;

            Assert.NotEqual(returnItem, new object());
        }

        [Fact]
        public void ReturnItem_EqualsSameInstance_ReturnsTrue()
        {
            var returnItem = ReturnItemBuilder.NewReturnItem;

            Assert.Equal(returnItem, returnItem);
        }

        [Fact]
        public void ReturnItem_EqualsReturnItemSameDetails_ReturnsTrue()
        {
            var returnItem = ReturnItemBuilder.NewReturnItem;
            var returnItem2 = ReturnItemBuilder.NewReturnItem;

            Assert.Equal(returnItem, returnItem2);
        }

        [Fact]
        public void ReturnItem_EqualsReturnItemDifferentObligationType_ReturnsFalse()
        {
            var returnItem = ReturnItemBuilder.WithObligationType(ObligationType.B2B);
            var returnItem2 = ReturnItemBuilder.WithObligationType(ObligationType.B2C);

            Assert.NotEqual(returnItem, returnItem2);
        }

        [Fact]
        public void ReturnItem_EqualsReturnItemDifferentWeeeCategory_ReturnsFalse()
        {
            var returnItem = ReturnItemBuilder.WithWeeeCategory(WeeeCategory.AutomaticDispensers);
            var returnItem2 = ReturnItemBuilder.WithWeeeCategory(WeeeCategory.ConsumerEquipment);

            Assert.NotEqual(returnItem, returnItem2);
        }

        [Fact]
        public void ReturnItem_EqualsReturnItemDifferentTonnage_ReturnsFalse()
        {
            var returnItem = ReturnItemBuilder.WithTonnage(5);
            var returnItem2 = ReturnItemBuilder.WithTonnage(10);

            Assert.NotEqual(returnItem, returnItem2);
        }

        private class ReturnItemBuilder
        {
            private ObligationType obligationType;
            private decimal tonnage;
            private WeeeCategory weeeCategory;

            public ReturnItemBuilder()
            {
                obligationType = ObligationType.B2C;
                tonnage = 100;
                weeeCategory = WeeeCategory.AutomaticDispensers;
            }

            public ReturnItem Build()
            {
                return new ReturnItem(obligationType, weeeCategory, tonnage);
            }

            public static ReturnItem NewReturnItem
            {
                get { return new ReturnItemBuilder().Build(); }
            }

            public static ReturnItem WithObligationType(ObligationType obligationType)
            {
                var builder = new ReturnItemBuilder();
                builder.obligationType = obligationType;

                return builder.Build();
            }

            public static ReturnItem WithWeeeCategory(WeeeCategory category)
            {
                var builder = new ReturnItemBuilder();
                builder.weeeCategory = category;

                return builder.Build();
            }

            public static ReturnItem WithTonnage(decimal tonnage)
            {
                var builder = new ReturnItemBuilder();
                builder.tonnage = tonnage;

                return builder.Build();
            }
        }
    }
}

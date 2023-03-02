namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using EA.Weee.Core.Helpers;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class CategoryValueTotalCalculatorTests
    {
        private ICategoryValueTotalCalculator calculator;

        public CategoryValueTotalCalculatorTests()
        {
            calculator = new CategoryValueTotalCalculator();
        }

        [Fact]
        public void CategoryValueTotalCalculator_IsDecoratedWith_SerializableAttribute()
        {
            typeof(CategoryValueTotalCalculator).Should()
                .BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void Total_OnPlusMinusSignInputValues_InvalidValuesIgnored()
        {
            var input = new List<string>() { "+2.000", "-3.000", "1.000" };
            var output = calculator.Total(input);

            Assert.Equal("1.000", output);
        }

        [Fact]
        public void Total_OnInputsWithMoreThan3DecimalPlaces_InvalidValuesIgnored()
        {
            var input = new List<string>() { "1.6687", "7.88845555", "1.000" };
            var output = calculator.Total(input);

            Assert.Equal("1.000", output);
        }

        [Fact]
        public void Total_OnNullInputValues_OutputIsZero()
        {
            var input = new List<string>() { null, null, null };
            var output = calculator.Total(input);

            Assert.Equal("0.000", output);
        }
    }
}

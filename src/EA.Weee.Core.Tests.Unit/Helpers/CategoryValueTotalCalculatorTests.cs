namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.Helpers;
    using FluentAssertions;
    using Xunit;

    public class CategoryValueTotalCalculatorTests
    {
        private ICategoryValueTotalCalculator calculator;

        public CategoryValueTotalCalculatorTests()
        {
            calculator = new CategoryValueTotalCalculator();
        }

        [Fact]
        public void Total_OnPlusMinusSignInputValues_InvalidValuesIgnored()
        {
            var input = new List<string>() { "+2.000", "-3.000", "1.000" };
            var output = calculator.Total(input);

            Assert.Equal(output, "1.000");
        }

        [Fact]
        public void Total_OnInputsWithMoreThan3DecimalPlaces_InvalidValuesIgnored()
        {
            var input = new List<string>() { "1.6687", "7.88845555", "1.000" };
            var output = calculator.Total(input);

            Assert.Equal(output, "1.000");
        }

        [Fact]
        public void Total_OnNullInputValues_OutputIsZero()
        {
            var input = new List<string>() { null, null, null };
            var output = calculator.Total(input);

            Assert.Equal(output, "0.000");
        }
    }
}

namespace EA.Weee.Core.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataReturns;
    using FluentAssertions;
    using Xunit;

    public class CategoryValuesTests
    {
        [Fact]
        public void CategoryValues_CategoriesShouldBePopulated()
        {
            var result = new CategoryValues();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)))
            {
                result.Count(c => c.Category.Equals(category)).Should().Be(1);
            }

            result.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void CategoryValues_CategorieValuesShouldBeNull()
        {
            var result = new CategoryValues();

            result.Count(c => c.HouseHold != null).Should().Be(0);
            result.Count(c => c.NonHouseHold != null).Should().Be(0);
        }
    }
}

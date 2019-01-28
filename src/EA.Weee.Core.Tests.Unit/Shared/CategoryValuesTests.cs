namespace EA.Weee.Core.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Helpers;
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

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                var foundCategory = result.FirstOrDefault(c => c.CategoryId == (int)category);
                foundCategory.Should().NotBeNull();
                foundCategory.CategoryDisplay.Should().Be(category.ToDisplayString<WeeeCategory>());
            }

            result.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void CategoryValues_CategoryValuesShouldBeNull()
        {
            var result = new CategoryValues();

            result.Count(c => c.HouseHold != null).Should().Be(0);
            result.Count(c => c.NonHouseHold != null).Should().Be(0);
            result.Count(c => c.Tonnage != null).Should().Be(0);
        }
    }
}

﻿namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using Core.Helpers;
    using DataReturns;
    using FluentAssertions;
    using System;
    using System.Linq;
    using Xunit;

    public class ObligatedCategoryValuesTests
    {
        [Fact]
        public void ObligatedCategoryValues_CategoriesShouldBePopulated()
        {
            var result = new ObligatedCategoryValues();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                var foundCategory = result.FirstOrDefault(c => c.CategoryId == (int)category);
                foundCategory.Should().NotBeNull();
                foundCategory.CategoryDisplay.Should().Be(category.ToDisplayString<WeeeCategory>());
            }

            result.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void NonObligatedCategoryValues_CategoriesShouldBePopulated()
        {
            var result = new NonObligatedCategoryValues();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                var foundCategory = result.FirstOrDefault(c => c.CategoryId == (int)category);
                foundCategory.Should().NotBeNull();
                foundCategory.CategoryDisplay.Should().Be(category.ToDisplayString<WeeeCategory>());
            }

            result.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void ObligatedCategoryValues_CategoryValuesShouldBeNull()
        {
            var result = new ObligatedCategoryValues();

            result.Count(c => c.B2C != null).Should().Be(0);
            result.Count(c => c.B2B != null).Should().Be(0);
        }

        [Fact]
        public void NonObligatedCategoryValues_CategoryValuesShouldBeNull()
        {
            var result = new NonObligatedCategoryValues();

            result.Count(c => c.Tonnage != null).Should().Be(0);
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Linq;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class NonObligatedValueViewModelTests
    {
        private readonly NonObligatedValuesViewModel model;

        public NonObligatedValueViewModelTests()
        {
            model = new NonObligatedValuesViewModel();
        }

        [Fact]
        public void GivenNonObligatedValuesViewModel_CategoriesShouldBePopulated()
        {
            model.CategoryValues.Should().NotBeNull();
            model.CategoryValues.Count().Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModel_TotalShouldBeZero()
        {
            model.Total.Should().Be("0.000");
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModelWithValues_TotalShouldBeCorrect()
        {
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = count + 1;
            }

            model.Total.Should().Be("105.000");
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModelWithDecimalValues_TotalShouldBeCorrect()
        {
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = count * 0.001m;
            }

            model.Total.Should().Be("0.091");
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModelWithNullValues_TotalShouldBeCorrect()
        {
            model.CategoryValues.ElementAt(2).Tonnage = 1;
            model.CategoryValues.ElementAt(4).Tonnage = 2;

            model.Total.Should().Be("3.000");
        }
    }
}

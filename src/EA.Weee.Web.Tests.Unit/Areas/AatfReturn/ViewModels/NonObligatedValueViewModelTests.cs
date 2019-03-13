﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Core.DataReturns;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class NonObligatedValueViewModelTests
    {
        private readonly NonObligatedValuesViewModel model;
        private readonly ICategoryValueTotalCalculator calculator;

        public NonObligatedValueViewModelTests()
        {
            calculator = new CategoryValueTotalCalculator();
            model = new NonObligatedValuesViewModel(calculator);
        }

        [Fact]
        public void GivenNonObligatedValuesViewModel_CategoriesShouldBePopulated()
        {
            model.CategoryValues.Should().NotBeNull();
            model.CategoryValues.Count().Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void GivenNonObligatedValuesViewModel_CalculatorShouldBeTypeCategoryValuesTotalCalculator()
        {
            var newModel = new NonObligatedValuesViewModel();
            newModel.GetCalculator().GetType().Should().Be(typeof(CategoryValueTotalCalculator));
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
                model.CategoryValues.ElementAt(count).Tonnage = (count + 1).ToString();
            }

            model.Total.Should().Be("105.000");
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModelWithDecimalValues_TotalShouldBeCorrect()
        {
            for (var count = 0; count < model.CategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Tonnage = (count * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            model.Total.Should().Be("0.091");
        }

        [Fact]
        public void Total_GivenNonObligatedValuesViewModelWithNullValues_TotalShouldBeCorrect()
        {
            model.CategoryValues.ElementAt(2).Tonnage = 1.ToString();
            model.CategoryValues.ElementAt(4).Tonnage = 2.ToString();

            model.Total.Should().Be("3.000");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("AAA")]
        [InlineData(" ")]
        [InlineData("")]
        public void Total_GivenNonObligatedValuesViewModelWithInvalidValues_TotalShouldBeCorrect(string value)
        {
            model.CategoryValues.ElementAt(2).Tonnage = value;
            model.CategoryValues.ElementAt(4).Tonnage = value;

            model.Total.Should().Be("0.000");
        }
    }
}

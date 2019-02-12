namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ObligatedReceivedViewModelTests
    {
        private readonly ObligatedReceivedViewModel viewModel;

        public ObligatedReceivedViewModelTests()
        {
            viewModel = new ObligatedReceivedViewModel();
        }

        [Fact]
        public void GivenObligatedReceivedViewModel_CategoriesShouldBePopulated()
        {
            viewModel.CategoryValues.Should().NotBeNull();
            viewModel.CategoryValues.Count().Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModel_TotalsShouldBeZero()
        {
            viewModel.HouseHoldTotal.Should().Be("0.000");
            viewModel.NonHouseHoldTotal.Should().Be("0.000");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithValues_TotalsShouldBeCorrect()
        {
            for (var count = 0; count < viewModel.CategoryValues.Count; count++)
            {
                viewModel.CategoryValues.ElementAt(count).HouseHold = (count + 1).ToString();
                viewModel.CategoryValues.ElementAt(count).NonHouseHold = (count + 1).ToString();
            }

            viewModel.HouseHoldTotal.Should().Be("105.000");
            viewModel.NonHouseHoldTotal.Should().Be("105.000");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithDecimalValues_TotalsShouldBeCorrect()
        {
            for (var count = 0; count < viewModel.CategoryValues.Count; count++)
            {
                viewModel.CategoryValues.ElementAt(count).HouseHold = (count * 0.001m).ToString(CultureInfo.InvariantCulture);
                viewModel.CategoryValues.ElementAt(count).NonHouseHold = (count * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            viewModel.HouseHoldTotal.Should().Be("0.091");
            viewModel.NonHouseHoldTotal.Should().Be("0.091");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithNullValues_TotalsShouldBeCorrect()
        {
            viewModel.CategoryValues.ElementAt(2).HouseHold = 1.ToString();
            viewModel.CategoryValues.ElementAt(4).HouseHold = 2.ToString();
            viewModel.CategoryValues.ElementAt(3).NonHouseHold = 3.ToString();
            viewModel.CategoryValues.ElementAt(5).NonHouseHold = 4.ToString();

            viewModel.HouseHoldTotal.Should().Be("3.000");
            viewModel.NonHouseHoldTotal.Should().Be("7.000");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("AAA")]
        [InlineData(" ")]
        [InlineData("")]
        public void Totals_GivenObligatedReceivedViewModelWithInvalidValues_TotalsShouldBeCorrect(string value)
        {
            viewModel.CategoryValues.ElementAt(2).HouseHold = value;
            viewModel.CategoryValues.ElementAt(4).HouseHold = value;
            viewModel.CategoryValues.ElementAt(3).HouseHold = value;
            viewModel.CategoryValues.ElementAt(5).HouseHold = value;

            viewModel.HouseHoldTotal.Should().Be("0.000");
            viewModel.NonHouseHoldTotal.Should().Be("0.000");
        }
    }
}

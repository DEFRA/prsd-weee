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
            viewModel.B2CTotal.Should().Be("0.000");
            viewModel.B2BTotal.Should().Be("0.000");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithValues_TotalsShouldBeCorrect()
        {
            for (var count = 0; count < viewModel.CategoryValues.Count; count++)
            {
                viewModel.CategoryValues.ElementAt(count).B2C = (count + 1).ToString();
                viewModel.CategoryValues.ElementAt(count).B2B = (count + 1).ToString();
            }

            viewModel.B2BTotal.Should().Be("105.000");
            viewModel.B2CTotal.Should().Be("105.000");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithDecimalValues_TotalsShouldBeCorrect()
        {
            for (var count = 0; count < viewModel.CategoryValues.Count; count++)
            {
                viewModel.CategoryValues.ElementAt(count).B2C = (count * 0.001m).ToString(CultureInfo.InvariantCulture);
                viewModel.CategoryValues.ElementAt(count).B2B = (count * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            viewModel.B2CTotal.Should().Be("0.091");
            viewModel.B2BTotal.Should().Be("0.091");
        }

        [Fact]
        public void Totals_GivenObligatedReceivedViewModelWithNullValues_TotalsShouldBeCorrect()
        {
            viewModel.CategoryValues.ElementAt(2).B2C = 1.ToString();
            viewModel.CategoryValues.ElementAt(4).B2C = 2.ToString();
            viewModel.CategoryValues.ElementAt(3).B2B = 3.ToString();
            viewModel.CategoryValues.ElementAt(5).B2B = 4.ToString();

            viewModel.B2CTotal.Should().Be("3.000");
            viewModel.B2BTotal.Should().Be("7.000");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("AAA")]
        [InlineData(" ")]
        [InlineData("")]
        public void Totals_GivenObligatedReceivedViewModelWithInvalidValues_TotalsShouldBeCorrect(string value)
        {
            viewModel.CategoryValues.ElementAt(2).B2C = value;
            viewModel.CategoryValues.ElementAt(4).B2C = value;
            viewModel.CategoryValues.ElementAt(3).B2B = value;
            viewModel.CategoryValues.ElementAt(5).B2B = value;

            viewModel.B2CTotal.Should().Be("0.000");
            viewModel.B2BTotal.Should().Be("0.000");
        }
    }
}

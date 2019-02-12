namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class ObligatedReceivedWeeeRequestCreatorTests
    {
        private readonly IObligatedReceivedWeeeRequestCreator requestCreator;

        public ObligatedReceivedWeeeRequestCreatorTests()
        {
            requestCreator = new ObligatedReceivedWeeeRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedReceivedViewModel(categoryValues);

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedReceivedViewModel(categoryValues);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].B2C = i.ToString();
                categoryValues[i].B2B = i.ToString();
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].HouseholdTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2C));
                request.CategoryValues[i].CategoryId.Should().Be(viewModel.CategoryValues[i].CategoryId);
                request.CategoryValues[i].NonHouseholdTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2B));
                request.CategoryValues[i].CategoryId.Should().Be(viewModel.CategoryValues[i].CategoryId);
            }
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModelWithDecimalValues_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedReceivedViewModel(categoryValues);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].B2C = (i * 0.001m).ToString(CultureInfo.InvariantCulture);
                categoryValues[i].B2B = (i * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].HouseholdTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2C));
                request.CategoryValues[i].NonHouseholdTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2B));
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ViewModelToRequested_GivenValidViewModelWithEmptyValues_CategoryValuesRequestPropertiesShouldBeMapped(string value)
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedReceivedViewModel(categoryValues);

            foreach (var c in categoryValues)
            {
                c.B2C = value;
                c.B2B = value;
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].HouseholdTonnage.Should().BeNull();
                request.CategoryValues[i].NonHouseholdTonnage.Should().BeNull();
            }
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_ViewModelPropertiesShouldBeMapped()
        {
            var model = new ObligatedReceivedViewModel()
            {
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid()
            };

            var request = requestCreator.ViewModelToRequest(model);

            request.OrganisationId.Should().Be(model.OrganisationId);
            request.ReturnId.Should().Be(model.ReturnId);
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class NonObligatedWeeRequestCreatorTests
    {
        private readonly INonObligatedWeeRequestCreator requestCreator;

        public NonObligatedWeeRequestCreatorTests()
        {
            requestCreator = new NonObligatedWeeRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var categoryValues = new NonObligatedCategoryValues();

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new NonObligatedCategoryValues();

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].Tonnage = i.ToString();
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].Tonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].Tonnage));
                request.CategoryValues[i].CategoryId.Should().Be(viewModel.CategoryValues[i].CategoryId);
            }
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModelWithDecimalValues_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new NonObligatedCategoryValues();

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].Tonnage = (i * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].Tonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].Tonnage));
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ViewModelToRequested_GivenValidViewModelWithEmptyValues_CategoryValuesRequestPropertiesShouldBeMapped(string value)
        {
            var categoryValues = new NonObligatedCategoryValues();

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            foreach (var t in categoryValues)
            {
                t.Tonnage = value;
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].Tonnage.Should().BeNull();
            }
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_CategoryValuesDcfShouldBeFalse()
        {
            var categoryValues = new NonObligatedCategoryValues();

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].Dcf.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ViewModelToRequested_GivenValidViewModel_CategoryValuesDcfShouldBeMapped(bool dcf)
        {
            var model = new NonObligatedValuesViewModel()
            {
                Dcf = dcf
            };
            var request = requestCreator.ViewModelToRequest(model);

            request.Dcf.Should().Be(model.Dcf);
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_ViewModelPropertiesShouldBeMapped()
        {
            var model = new NonObligatedValuesViewModel()
            {
                Dcf = true,
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid()
            };

            var request = requestCreator.ViewModelToRequest(model);

            request.OrganisationId.Should().Be(model.OrganisationId);
            request.ReturnId.Should().Be(model.ReturnId);
            request.Dcf.Should().Be(model.Dcf);
        }
    }
}

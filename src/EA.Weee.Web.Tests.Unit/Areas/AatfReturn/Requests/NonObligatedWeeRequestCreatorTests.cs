namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Web.OAuth;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Owin.Security;
    using System;
    using System.Globalization;
    using System.Security;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
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
    }
}

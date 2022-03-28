﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Web.Areas.Aatf.Requests;
    using Weee.Requests.AatfReturn.Obligated;
    using Xunit;

    public class ObligatedReceivedWeeeRequestCreatorTests
    {
        private readonly IObligatedReceivedWeeeRequestCreator requestCreator;
        private readonly ICategoryValueTotalCalculator calculator;

        public ObligatedReceivedWeeeRequestCreatorTests()
        {
            requestCreator = new ObligatedReceivedWeeeRequestCreator();
            calculator = new CategoryValueTotalCalculator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedViewModel(categoryValues, calculator);

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequest_GivenViewModelIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => requestCreator.ViewModelToRequest(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedViewModel(categoryValues, calculator);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].B2C = i.ToString();
                categoryValues[i].B2B = i.ToString();
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].FirstTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2C));
                request.CategoryValues[i].CategoryId.Should().Be(viewModel.CategoryValues[i].CategoryId);
                request.CategoryValues[i].SecondTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2B));
                request.CategoryValues[i].CategoryId.Should().Be(viewModel.CategoryValues[i].CategoryId);
            }
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModelWithDecimalValues_CategoryValuesRequestPropertiesShouldBeMapped()
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedViewModel(categoryValues, calculator);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].B2C = (i * 0.001m).ToString(CultureInfo.InvariantCulture);
                categoryValues[i].B2B = (i * 0.001m).ToString(CultureInfo.InvariantCulture);
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].FirstTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2C));
                request.CategoryValues[i].SecondTonnage.Should().Be(Convert.ToDecimal(viewModel.CategoryValues[i].B2B));
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ViewModelToRequest_GivenValidViewModelWithEmptyValues_CategoryValuesRequestPropertiesShouldBeMapped(string value)
        {
            var categoryValues = new ObligatedCategoryValues();

            var viewModel = new ObligatedViewModel(categoryValues, calculator);

            foreach (var c in categoryValues)
            {
                c.B2C = value;
                c.B2B = value;
            }

            var request = requestCreator.ViewModelToRequest(viewModel);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                request.CategoryValues[i].FirstTonnage.Should().BeNull();
                request.CategoryValues[i].SecondTonnage.Should().BeNull();
            }
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_ViewModelPropertiesShouldBeMapped()
        {
            var model = new ObligatedViewModel(calculator)
            {
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid()
            };

            var request = requestCreator.ViewModelToRequest(model) as AddObligatedReceived;

            request.OrganisationId.Should().Be(model.OrganisationId);
            request.ReturnId.Should().Be(model.ReturnId);
        }

        [Fact]
        public void ViewModelToRequest_GivenEditViewModel_RequestTypeShouldBeEdit()
        {
            var model = new ObligatedViewModel(calculator);
            model.CategoryValues.ElementAt(0).Id = Guid.NewGuid();

            var request = requestCreator.ViewModelToRequest(model);

            request.Should().BeOfType<EditObligatedReceived>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEditViewModel_CategoryValuesShouldBeMapped()
        {
            var model = new ObligatedViewModel(calculator)
            {
                CategoryValues = new List<ObligatedCategoryValue>() { new ObligatedCategoryValue() }
            };

            model.CategoryValues.ElementAt(0).Id = Guid.NewGuid();

            var request = requestCreator.ViewModelToRequest(model);

            request.CategoryValues.Should().NotBeNull();
            request.CategoryValues.Count().Should().Be(1);
        }

        [Fact]
        public void ViewModelToRequest_GivenAddViewModel_RequestTypeShouldBeAdd()
        {
            var model = new ObligatedViewModel(calculator);

            var request = requestCreator.ViewModelToRequest(model);

            request.Should().BeOfType<AddObligatedReceived>();
        }
    }
}

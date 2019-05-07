namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Attributes;
    using Validation;
    using Xunit;

    public class SelectReportOptionsViewModelTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public void SelectReportOptionsViewModel_GivenSelectedValueIsYesOrNo_IsValid(string selectedValue)
        {
            var viewModel = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>()) { DcfSelectedValue = selectedValue };
            var context = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void SelectReportOptionsViewModel_ClassHasValidatorAttribute()
        {
            var t = typeof(SelectReportOptionsViewModel);
            var customAttribute = t.GetCustomAttribute(typeof(ValidatorAttribute)) as FluentValidation.Attributes.ValidatorAttribute;

            customAttribute.ValidatorType.Should().Be(typeof(SelectReportOptionsViewModelValidator));
        }

        [Fact]
        public void SelectReportOptionsViewModel_GivenZeroSelectionOptions_HasSelectionOptionsIsFalse()
        {
            var viewModel = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>()) { SelectedOptions = new List<int>() };

            viewModel.HasSelectedOptions.Should().Be(false);
        }

        [Fact]
        public void SelectReportOptionsViewModel_GivenNullSelectionOptions_HasSelectionOptionsIsFalse()
        {
            var viewModel = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>()) { SelectedOptions = null };

            viewModel.HasSelectedOptions.Should().Be(false);
        }

        [Fact]
        public void SelectReportOptionsViewModel_GivenAnySelectionOptions_HasSelectionOptionsIsTrue()
        {
            var viewModel = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>()) { SelectedOptions = new List<int>() { 1 } };

            viewModel.HasSelectedOptions.Should().Be(true);
        }
    }
}
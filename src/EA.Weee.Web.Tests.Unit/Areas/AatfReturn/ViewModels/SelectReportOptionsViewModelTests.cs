namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using AutoFixture;
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
        private readonly Fixture fixture;

        public SelectReportOptionsViewModelTests()
        {
            fixture = new Fixture();
        }

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
        public void SelectReportOptionsViewModel_GivenZeroSelectionOptions_HasSelectedOptionsIsFalse()
        {
            var viewModel = GetDefaultViewModel();

            viewModel.HasSelectedOptions.Should().Be(false);
        }

        [Fact]
        public void SelectReportOptionsViewModel_GivenNullSelectionOptions_HasSelectedOptionsIsFalse()
        {
            var viewModel = GetDefaultViewModel();

            viewModel.HasSelectedOptions.Should().Be(false);
        }

        [Fact]
        public void SelectReportOptionsViewModel_GivenAnySelectionOptions_HasSelectedOptionsIsTrue()
        {
            var viewModel = GetDefaultViewModel();
            viewModel.ReportOnQuestions = new List<ReportOnQuestion>() {fixture.Build<ReportOnQuestion>().With(r => r.Selected, true).Create() };

            viewModel.HasSelectedOptions.Should().Be(true);
        }

        [Fact]
        public void DeSelectedOptions_GivenNoDeselectedReportQuestions_EmptyShouldBeReturned()
        {
            var viewModel = GetDefaultViewModel();

            viewModel.DeSelectedOptions.Should().BeEmpty();
        }

        [Fact]
        public void DeSelectedOptions_GivenDeselectedReportQuestions_ReturnShouldContainDeSelectedReportQuestions()
        {
            var viewModel = GetDefaultViewModel();

            var deselected1 = fixture.Build<ReportOnQuestion>().With(r => r.OriginalSelected, true).With(r => r.Selected, false).With(r => r.Id, 1).Create();
            var deselected2 = fixture.Build<ReportOnQuestion>().With(r => r.OriginalSelected, true).With(r => r.Selected, false).With(r => r.Id, 2).Create();
            var selected = fixture.Build<ReportOnQuestion>().With(r => r.OriginalSelected, false).With(r => r.Selected, true).With(r => r.Id, 3).Create();

            viewModel.ReportOnQuestions = new List<ReportOnQuestion>()
            {
                deselected1, deselected2, selected
            };

            viewModel.DeSelectedOptions.Should().Contain(deselected1.Id);
            viewModel.DeSelectedOptions.Should().Contain(deselected2.Id);
            viewModel.DeSelectedOptions.Should().NotContain(selected.Id);
            viewModel.DeSelectedOptions.Count.Should().Be(2);
        }

        [Fact]
        public void SelectedOptions_GivenSelectedReportQuestions_ReturnShouldContainSelectedReportQuestionsAndExcludeDcfSelection()
        {
            var viewModel = GetDefaultViewModel();

            var selected1 = fixture.Build<ReportOnQuestion>().With(r => r.Selected, true).With(r => r.Id, 1).Create();
            var selected2 = fixture.Build<ReportOnQuestion>().With(r => r.Selected, true).With(r => r.Id, 2).Create();
            var dcfSelected = fixture.Build<ReportOnQuestion>().With(r => r.Selected, true).With(r => r.Id, (int)ReportOnQuestionEnum.NonObligatedDcf).Create();

            viewModel.ReportOnQuestions = new List<ReportOnQuestion>()
            {
                selected1, selected2, dcfSelected
            };

            viewModel.SelectedOptions.Should().Contain(selected1.Id);
            viewModel.SelectedOptions.Should().Contain(selected2.Id);
            viewModel.DeSelectedOptions.Should().NotContain(dcfSelected.Id);
            viewModel.SelectedOptions.Count.Should().Be(2);
        }

        [Fact]
        public void SelectedOptions_GivenNoDeselectedReportQuestions_EmptyShouldBeReturned()
        {
            var viewModel = GetDefaultViewModel();

            viewModel.SelectedOptions.Should().BeEmpty();
        }

        [Fact]
        public void DcfSelectedValue_GivenNullValue_DcfQuestionSelectedShouldNotBeSet()
        {
            var viewModel = GetDefaultViewModelWithDcfQuestion();

            viewModel.DcfSelectedValue = null;
            viewModel.DcfQuestion.Selected.Should().BeFalse();
        }

        [Fact]
        public void DcfSelectedValue_GivenYesValue_DcfQuestionSelectedShouldBeTrue()
        {
            var viewModel = GetDefaultViewModelWithDcfQuestion();

            viewModel.DcfSelectedValue = "Yes";
            viewModel.DcfQuestion.Selected.Should().BeTrue();
        }

        [Fact]
        public void DcfSelectedValue_GivenNoValue_DcfQuestionSelectedShouldBeFalse()
        {
            var viewModel = GetDefaultViewModelWithDcfQuestion();

            viewModel.DcfSelectedValue = "No";
            viewModel.DcfQuestion.Selected.Should().BeFalse();
        }

        [Fact]
        public void DcfQuestion_ShouldReturnDcfQuestion()
        {
            var viewModel = GetDefaultViewModelWithDcfQuestion();

            viewModel.DcfQuestion.Id.Should().Be((int)ReportOnQuestionEnum.NonObligatedDcf);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DcfQuestionSelected_GivenDcfQuestionIsOrIsNotSelected_CorrectResultShouldBeReturned(bool selection)
        {
            var model = GetDefaultViewModel();

            var dcf = fixture.Build<ReportOnQuestion>().With(r => r.Id, (int)ReportOnQuestionEnum.NonObligatedDcf).With(r => r.Selected, selection).Create();
            model.ReportOnQuestions = new List<ReportOnQuestion>()
            {
                dcf
            };

            model.DcfQuestionSelected.Should().Be(selection);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NonObligatedQuestionSelected_GivenNonObligatedQuestionIsOrIsNotSelected_CorrectResultShouldBeReturned(bool selection)
        {
            var model = GetDefaultViewModel();

            var nonObligatedQuestion = fixture.Build<ReportOnQuestion>().With(r => r.Id, (int)ReportOnQuestionEnum.NonObligated).With(r => r.Selected, selection).Create();
            model.ReportOnQuestions = new List<ReportOnQuestion>()
            {
                nonObligatedQuestion
            };

            model.NonObligatedQuestionSelected.Should().Be(selection);
        }

        [Fact]
        public void YesValue_YesShouldBeReturned()
        {
            var model = GetDefaultViewModel();

            model.YesValue.Should().Be("Yes");
        }

        [Fact]
        public void NoValue_NoShouldBeReturned()
        {
            var model = GetDefaultViewModel();

            model.NoValue.Should().Be("No");
        }

        private SelectReportOptionsViewModel GetDefaultViewModel()
        {
            return new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>());
        }

        private SelectReportOptionsViewModel GetDefaultViewModelWithDcfQuestion()
        {
            var model = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Fake<ReturnData>(), A.Dummy<int>());

            var dcf = fixture.Build<ReportOnQuestion>().With(r => r.Id, (int)ReportOnQuestionEnum.NonObligatedDcf).Create();
            model.ReportOnQuestions = new List<ReportOnQuestion>()
            {
                dcf
            };

            return model;
        }
    }
}
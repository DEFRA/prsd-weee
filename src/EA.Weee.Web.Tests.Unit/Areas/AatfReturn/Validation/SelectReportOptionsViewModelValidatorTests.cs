namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Results;
    using Xunit;

    public class SelectReportOptionsViewModelValidatorTests
    {
        private SelectReportOptionsViewModelValidator validator;
        private const int DcfQuestionId = 2;

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueNotSelected_ViewModelErrorPropertyShouldBeTrue()
        {
            var model = GenerateViewModel();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            model.ReportOnQuestions.First(r => r.Id == DcfQuestionId).HasError.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndNullDcfValueSelected_ErrorShouldOccur()
        {
            var model = GenerateViewModel();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsYes_ErrorShouldNotOccur()
        {
            var model = GenerateViewModel();

            model.DcfSelectedValue = "Yes";

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
            model.ReportOnQuestions.Any(r => r.HasError).Should().BeFalse();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsNo_ErrorShouldNotOccur()
        {
            var model = GenerateViewModel();

            model.DcfSelectedValue = "No";

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsYes_ErrorShouldBeValid()
        {
            var model = GenerateViewModel();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.Errors.Should().Contain(v =>
                v.ErrorMessage.Equals("You must tell us whether any of the non-obligated WEEE was retained by a DCF"));
            validationResult.Errors.Should().Contain(v =>
                v.PropertyName.Equals("Option-1"));
        }

        private SelectReportOptionsViewModel GenerateViewModel()
        {
            var nonObligatedQuestion = new ReportOnQuestion(1, A.Dummy<string>(), A.Dummy<string>(), default(int));
            var dcfQuestion = new ReportOnQuestion(DcfQuestionId, A.Dummy<string>(), A.Dummy<string>(), 1);

            var model = new SelectReportOptionsViewModel()
            {
                ReportOnQuestions = new List<ReportOnQuestion>() { nonObligatedQuestion, dcfQuestion },
                SelectedOptions = new List<int>() { nonObligatedQuestion.Id }
            };
            return model;
        }
    }
}

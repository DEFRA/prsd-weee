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

        [Fact]
        public void RuleFor_NonObligatedSelectedAndNullDcfValueSelected_ErrorShouldOccur()
        {
            SelectReportOptionsViewModel model = GenerateViewModel();

            validator = new SelectReportOptionsViewModelValidator();
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsYes_ErrorShouldNotOccur()
        {
            SelectReportOptionsViewModel model = GenerateViewModel();

            model.DcfSelectedValue = "Yes";

            validator = new SelectReportOptionsViewModelValidator();
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsNo_ErrorShouldNotOccur()
        {
            SelectReportOptionsViewModel model = GenerateViewModel();

            model.DcfSelectedValue = "No";

            validator = new SelectReportOptionsViewModelValidator();
            ValidationResult validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }

        private SelectReportOptionsViewModel GenerateViewModel()
        {
            var nonObligatedQuestion = new ReportOnQuestion(1, A.Dummy<string>(), A.Dummy<string>(), default(int));
            var dcfQuestion = new ReportOnQuestion(2, A.Dummy<string>(), A.Dummy<string>(), 1);

            var model = new SelectReportOptionsViewModel()
            {
                ReportOnQuestions = new List<ReportOnQuestion>() { nonObligatedQuestion, dcfQuestion },
                SelectedOptions = new List<int>() { nonObligatedQuestion.Id }
            };
            return model;
        }
    }
}

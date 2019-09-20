namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Validation
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Xunit;

    public class SelectReportOptionsViewModelValidatorTests
    {
        private readonly Fixture fixture;
        private SelectReportOptionsViewModelValidator validator;

        public SelectReportOptionsViewModelValidatorTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueNotSelected_ViewModelErrorPropertyShouldBeTrue()
        {
            var model = GenerateInvalidNonObligated();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            model.ReportOnQuestions.First(r => r.Id == (int)ReportOnQuestionEnum.NonObligatedDcf).HasError.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndNullDcfValueSelected_ErrorShouldOccur()
        {
            var model = GenerateInvalidNonObligated();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsYes_ErrorShouldNotOccur()
        {
            var model = GenerateInvalidNonObligated();

            model.DcfSelectedValue = "Yes";

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
            model.ReportOnQuestions.Any(r => r.HasError).Should().BeFalse();
        }

        [Fact]
        public void RuleFor_GivenSelectedValueAndNonObligatedSelectedAndDcfValueIsNo_ErrorShouldNotOccur()
        {
            var model = GenerateInvalidNonObligated();

            model.DcfSelectedValue = "No";

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_NonObligatedSelectedAndDcfValueIsYes_ErrorShouldBeValid()
        {
            var model = GenerateInvalidNonObligated();

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.Errors.Should().Contain(v =>
                v.ErrorMessage.Equals("You must tell us whether any of the non-obligated WEEE was retained by a DCF"));
            validationResult.Errors.Should().Contain(v =>
                v.PropertyName.Equals("Option-4"));
        }

        [Fact]
        public void RuleFor_GivenNoSelectedItems_ErrorShouldBeValid()
        {
            var question = fixture.Build<ReportOnQuestion>()
                .With(r => r.Id, (int)ReportOnQuestionEnum.WeeeReceived)
                .With(r => r.Selected, false)
                .Create();

            var model = new SelectReportOptionsViewModel()
            {
                ReportOnQuestions = new List<ReportOnQuestion>() { question },
            };

            validator = new SelectReportOptionsViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.Errors.Should().Contain(v =>
                v.ErrorMessage.Equals("You must select at least one reporting option, unless you have no data to report"));
            validationResult.Errors.Should().Contain(v =>
                v.PropertyName.Equals("hasSelectedOptions"));
        }

        private SelectReportOptionsViewModel GenerateInvalidNonObligated()
        {
            var nonObligatedQuestion = fixture.Build<ReportOnQuestion>()
                .With(r => r.Id, (int)ReportOnQuestionEnum.NonObligated)
                .With(r => r.HasError, false)
                .With(r => r.Selected, true)
                .Create();
            
            var nonObligatedDcfQuestion = fixture.Build<ReportOnQuestion>()
                .With(r => r.Id, (int)ReportOnQuestionEnum.NonObligatedDcf)
                .With(r => r.HasError, false)
                .With(r => r.Selected, false)
                .Create();

            var selectedQuestion = fixture.Build<ReportOnQuestion>()
                .With(r => r.Id, (int)ReportOnQuestionEnum.WeeeSentOn)
                .With(r => r.HasError, false)
                .With(r => r.Selected, true).Create();

            var model = new SelectReportOptionsViewModel()
            {
                ReportOnQuestions = new List<ReportOnQuestion>() { nonObligatedQuestion, selectedQuestion, nonObligatedDcfQuestion },
            };
            return model;
        }
    }
}

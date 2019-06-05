namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Validation
{
    using System;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentAssertions;
    using Xunit;

    public class ApprovalDateValidatorTests
    {
        private ApprovalDateValidator validator;

        [Theory]
        [InlineData("2018/12/31")]
        [InlineData("2020/1/1")]
        public void RuleFor_ApprovalDateOutOfBounds_ErrorShouldOccur(string input)
        {
            var model = GenerateViewModel();
            model.ApprovalDate = Convert.ToDateTime(input);

            validator = new ApprovalDateValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be("Approval date must be between 01/01/2019 and 31/12/2019");
        }

        [Theory]
        [InlineData("2019/1/1")]
        [InlineData("2019/12/31")]
        [InlineData("2019/7/1")]
        public void RuleFor_ApprovalDateWithinBounds_ErrorShouldNotOccur(string input)
        {
            var model = GenerateViewModel();
            model.ApprovalDate = Convert.ToDateTime(input);

            validator = new ApprovalDateValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Count.Should().Be(0);
        }

        private FacilityViewModelBase GenerateViewModel()
        {
            var model = new AddAatfViewModel
            {
                ComplianceYear = 2019
            };
            return model as FacilityViewModelBase;
        }
    }
}

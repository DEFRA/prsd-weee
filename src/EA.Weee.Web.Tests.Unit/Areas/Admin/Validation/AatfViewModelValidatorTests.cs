namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Validation
{
    using System;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentAssertions;
    using Xunit;

    public class AatfViewModelValidatorTests
    {
        private AatfViewModelValidator validator;

        [Theory]
        [InlineData("2018/12/31")]
        [InlineData("2020/1/1")]
        public void RuleFor_ApprovalDateOutOfBounds_ErrorShouldOccur(string input)
        {
            var model = GenerateViewModel();
            model.ApprovalDate = Convert.ToDateTime(input);

            validator = new AatfViewModelValidator();
            var validationResult = validator.Validate(model);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        private AatfViewModelBase GenerateViewModel()
        {
            var model = new AddAatfViewModel()
            {
                ComplianceYear = (Int16)2019
            };
            return model as AatfViewModelBase;
        }
    }
}

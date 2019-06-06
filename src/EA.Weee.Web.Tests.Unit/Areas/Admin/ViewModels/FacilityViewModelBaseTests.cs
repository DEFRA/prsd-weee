namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FluentAssertions;
    using Xunit;

    public class FacilityViewModelBaseTests
    {
        [Theory]
        [InlineData("2018/12/31")]
        [InlineData("2020/1/1")]
        public void RuleFor_ApprovalDateOutOfBounds_ErrorShouldOccur(string input)
        {
            var model = new OverriddenFacilityViewModelBase { ApprovalDate = Convert.ToDateTime(input), ComplianceYear = 2019 };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(1);
            validationResult.First().ErrorMessage.Should().Be("Approval date must be between 01/01/2019 and 31/12/2019");
        }

        [Theory]
        [InlineData("2019/1/1")]
        [InlineData("2019/12/31")]
        [InlineData("2019/7/1")]
        public void RuleFor_ApprovalDateWithinBounds_ErrorShouldNotOccur(string input)
        {
            var model = new OverriddenFacilityViewModelBase { ApprovalDate = Convert.ToDateTime(input), ComplianceYear = 2019 };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(0);
        }
    }
}

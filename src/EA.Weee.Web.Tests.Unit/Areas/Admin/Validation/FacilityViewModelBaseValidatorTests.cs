namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Validation
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class FacilityViewModelBaseValidatorTests
    {
        private readonly FacilityViewModelBaseValidator validator;
        private readonly FacilityViewModelBase model;
        private readonly IWeeeClient apiClient;

        public FacilityViewModelBaseValidatorTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.model = A.Fake<FacilityViewModelBase>();

            validator = new FacilityViewModelBaseValidator(A.Dummy<string>(), () => apiClient, model);
        }

        [Fact]
        public void RuleFor_RequestReturnsTrue_IsValidShouldBeFalse()
        {
            var exists = true;
            var viewModel = new AddAatfViewModel()
            {
                ApprovalNumber = "WEE/AZ1234AZ/ATF"
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<CheckApprovalNumberIsUnique>.That.Matches(c => c.ApprovalNumber == viewModel.ApprovalNumber))).Returns(exists);

            var validationResult = validator.Validate(viewModel);

            validationResult.IsValid.Should().Be(false);
        }

        [Fact]
        public void RuleFor_ApiCallMustHaveHappened()
        {
            var viewModel = new AddAatfViewModel()
            {
                ApprovalNumber = "WEE/AZ1234AZ/ATF"
            };

            var validationResult = validator.Validate(viewModel);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<CheckApprovalNumberIsUnique>.That.Matches(c => c.ApprovalNumber == viewModel.ApprovalNumber))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

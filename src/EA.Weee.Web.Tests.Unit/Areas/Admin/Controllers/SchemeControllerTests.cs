namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using FakeItEasy;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Weee.Requests.Scheme;
    using Xunit;

    public class SchemeControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeClient weeeFakeClient;

        public SchemeControllerTests()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            apiClient = () => weeeClient;

            weeeFakeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async Task ManageSchemesPost_AllGood_ReturnsManageSchemeRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = new SchemeController(apiClient);

            var result = await controller.ManageSchemes(new ManageSchemesViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("EditScheme", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["schemeId"]);
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            var controller = new SchemeController(apiClient);
            controller.ModelState.AddModelError(string.Empty, "Some error");

            var result = await controller.ManageSchemes(new ManageSchemesViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetEditScheme_ModelWithNoError_ReturnsView()
        {
            var controller = new SchemeController(apiClient);

            var viewResult = await controller.EditScheme(Guid.NewGuid());

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_ReturnsViewWithError()
        {
            var controller = new SchemeController(apiClient);
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var viewResult = await controller.EditScheme(new SchemeViewModel());

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData("Wee/AB1234CD/SCH")]
        [InlineData("WEE/AB1234CD/sch")]
        [InlineData("WEE/AB1234CD/123")]
        [InlineData("WEE/891234CD/SCH")]
        [InlineData("WEE/AB1DF4CD/SCH")]
        [InlineData("WEE/AB123482/SCH")]
        public async void PostEditScheme_ModelWithInCorrectApprovalNumber_ReturnsViewWithError(string approvalNumber)
        {
            var controller = new SchemeController(apiClient);
            var model = new SchemeViewModel
            {
                ApprovalNumber = approvalNumber,
                CompetentAuthorities = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                CompetentAuthorityName = "Any name",
                IbisCustomerReference = "Any value",
                ObligationType = ObligationType.B2B,
                ObligationTypeSelectList = new List<SelectListItem>(),
                SchemeName = "Any value"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var viewResult = await controller.EditScheme(model);

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.False(isModelStateValid);
        }

        [Theory]
        [InlineData("WEE/AB1234CD/SCH")]
        [InlineData("WEE/DE8562FG/SCH")]
        public async void PostEditScheme_ModelWithCorrectApprovalNumber_ReturnsView(string approvalNumber)
        {
            var controller = new SchemeController(apiClient);
            var model = new SchemeViewModel
            {
                ApprovalNumber = approvalNumber,
                CompetentAuthorities = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                CompetentAuthorityName = "Any name",
                IbisCustomerReference = "Any value",
                ObligationType = ObligationType.B2B,
                ObligationTypeSelectList = new List<SelectListItem>(),
                SchemeName = "Any value"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var viewResult = await controller.EditScheme(model);

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.True(isModelStateValid);
        }

        [Fact]
        public async void PostEditScheme_OldApprovalNumberAndApprovalNumberNotMatch_MustVerifyApprovalNumberExists()
        {
            var controller = new SchemeController(apiClient);

            var scheme = new SchemeViewModel
            {
                OldApprovalNumber = "WEE/AD1234DC/SCH",
                ApprovalNumber = "WEE/ZZ3456EE/SCH",
                SchemeName = "Any value",
                ObligationType = ObligationType.B2B,
                CompetentAuthorityId = Guid.NewGuid(),
                IbisCustomerReference = "Any value"
            };

            await controller.EditScheme(scheme);

            A.CallTo(() => apiClient.Invoke().SendAsync(A<string>._, A<VerifyApprovalNumberExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => apiClient.Invoke().SendAsync(A<string>._, A<UpdateSchemeInformation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

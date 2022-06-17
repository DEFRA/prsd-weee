namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mediator;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Areas.Admin.ViewModels.Home;
    using Web.ViewModels.Shared;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient apiClient;

        public HomeControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(HomeController).Should().BeDerivedFrom<AdminController>();
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Pending)]
        [InlineData(UserStatus.Rejected)]
        public async void HttpGet_Index_IfUserIsNotActive_ShouldRedirectToInternalUserAuthorizationRequired(
            UserStatus userStatus)
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<IRequest<UserStatus>>._))
                .Returns(userStatus);

            var result = await HomeController().Index();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Account", routeValues["controller"]);
            Assert.Equal(userStatus, routeValues["userStatus"]);
        }

        [Fact]
        public async void HttpGet_Index_IfUserIsActive_ShouldRedirectToChooseActivity()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<IRequest<UserStatus>>._))
                .Returns(UserStatus.Active);

            var result = await HomeController().Index();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public void HttpGet_ChooseActivity_ShouldReturnsChooseActivityView()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Role, "InternalAdmin")
            }, "TestAuthentication"));
            var controller = HomeController();
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);

            var result = controller.ChooseActivity();
            var viewResult = ((ViewResult)result);
            Assert.Equal("ChooseActivity", viewResult.ViewName);
        }

        [Fact]
        public void HttpGet_ChooseActivity_WithStandardUserAndFeatureEnabled_ViewModelPossibleValuesShouldBeInCorrectOrder()
        {
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(true);
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "InternalUser") }, "TestAuthentication"));
            
            HomeController controller = new HomeController(() => apiClient, configuration);
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);
            var result = controller.ChooseActivity() as ViewResult;
            var model = result.Model as RadioButtonStringCollectionViewModel;

            // Note: in this case InternalUserActivity.ManagePcsObligations should not be listed
            model.PossibleValues[0].Should().Be(InternalUserActivity.ManageScheme);
            model.PossibleValues[1].Should().Be(InternalUserActivity.SubmissionsHistory);
            model.PossibleValues[2].Should().Be(InternalUserActivity.ProducerDetails);
            model.PossibleValues[3].Should().Be(InternalUserActivity.ManageEvidenceNotes);
            model.PossibleValues[4].Should().Be(InternalUserActivity.ManagePcsCharges);
            model.PossibleValues[5].Should().Be(InternalUserActivity.ManageAatfs);
            model.PossibleValues[6].Should().Be(InternalUserActivity.ManageAes);
            model.PossibleValues[7].Should().Be(InternalUserActivity.ManageUsers);
            model.PossibleValues[8].Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public void HttpGet_ChooseActivity_WithAdminUserAndFeatureEnabled_ViewModelPossibleValuesShouldBeInCorrectOrder()
        {
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(true);
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "InternalAdmin") }, "TestAuthentication"));
            HomeController controller = new HomeController(() => apiClient, configuration);
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);
            var result = controller.ChooseActivity() as ViewResult;
            var model = result.Model as RadioButtonStringCollectionViewModel;

            model.PossibleValues[0].Should().Be(InternalUserActivity.ManageScheme);
            model.PossibleValues[1].Should().Be(InternalUserActivity.SubmissionsHistory);
            model.PossibleValues[2].Should().Be(InternalUserActivity.ProducerDetails);
            model.PossibleValues[3].Should().Be(InternalUserActivity.ManageEvidenceNotes);
            model.PossibleValues[4].Should().Be(InternalUserActivity.ManagePcsObligations);
            model.PossibleValues[5].Should().Be(InternalUserActivity.ManagePcsCharges);
            model.PossibleValues[6].Should().Be(InternalUserActivity.ManageAatfs);
            model.PossibleValues[7].Should().Be(InternalUserActivity.ManageAes);
            model.PossibleValues[8].Should().Be(InternalUserActivity.ManageUsers);
            model.PossibleValues[9].Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public void HttpPost_ChooseActivity_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = HomeController();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "InternalAdmin")
            }, "TestAuthentication"));
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);
            controller.ModelState.AddModelError("Key", "Any error");

            var result = controller.ChooseActivity(A.Dummy<ChooseActivityViewModel>());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData(InternalUserActivity.ManageUsers, "Index")]
        [InlineData(InternalUserActivity.ManageScheme, "ManageSchemes")]
        [InlineData(InternalUserActivity.ManageAatfs, "ManageAatfs")]
        [InlineData(InternalUserActivity.ManageAes, "ManageAatfs")]
        [InlineData(InternalUserActivity.ProducerDetails, "Search")]
        [InlineData(InternalUserActivity.ManageEvidenceNotes, "Index")]
        [InlineData(InternalUserActivity.SubmissionsHistory, "ChooseSubmissionType")]
        [InlineData(InternalUserActivity.ViewReports, "ChooseReport")]
        [InlineData(InternalUserActivity.ManagePcsCharges, "SelectAuthority")]
        [InlineData(InternalUserActivity.ManagePcsObligations, "SelectAuthority")]
        public void HttpPost_ChooseActivity_RedirectsToCorrectControllerAction(string selection, string action)
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(true);
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);
            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedValue = selection;
            HomeController controller = new HomeController(() => apiClient, configuration);

            // Act
            ActionResult result = controller.ChooseActivity(viewModel);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);

            if (selection == InternalUserActivity.ManagePcsObligations)
            {
                Assert.Equal("Obligations", redirectToRouteResult.RouteValues["controller"]);
            }

            if (selection == InternalUserActivity.ManageEvidenceNotes)
            {
                Assert.Equal("AdminHolding", redirectToRouteResult.RouteValues["controller"]);
            }

            if (selection == InternalUserActivity.ManageAatfs)
            {
                Assert.Equal(FacilityType.Aatf, redirectToRouteResult.RouteValues["facilityType"]);
                Assert.Equal("ManageAatfs", redirectToRouteResult.RouteValues["action"]);
                Assert.Equal("Aatf", redirectToRouteResult.RouteValues["controller"]);
            }

            if (selection == InternalUserActivity.ManageAes)
            {
                Assert.Equal(FacilityType.Ae, redirectToRouteResult.RouteValues["facilityType"]);
                Assert.Equal("ManageAatfs", redirectToRouteResult.RouteValues["action"]);
                Assert.Equal("Aatf", redirectToRouteResult.RouteValues["controller"]);
            }
        }

        /// <summary>
        /// This test ensures that the "ManagePcsCharges" option is considered invalid for the POST ChoooseActivity
        /// action when the configuration has "EnabledInvoicing" set to false.
        /// </summary>
        [Fact]
        public void PostChooseActivity_SelectedManagePcsChargesWhenConfigSetToFalse_ThrowsInvalidOperationException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableInvoicing).Returns(false);

            HomeController controller = new HomeController(() => apiClient, configuration);

            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedValue = InternalUserActivity.ManagePcsCharges.ToString();

            // Act
            Func<ActionResult> testCode = () => controller.ChooseActivity(viewModel);

            // Assert
            Assert.Throws<InvalidOperationException>(testCode);
        }

        /// <summary>
        /// This test ensures that the "ManagePcsCharges" option is considered invalid for the POST ChoooseActivity
        /// action when the configuration has "EnabledInvoicing" set to false.
        /// </summary>
        [Fact]
        public void PostChooseActivity_SelectedManagePcsObligationsWhenConfigSetToFalse_ThrowsInvalidOperationException()
        {
            // Arrange
            HomeController controller = new HomeController(() => apiClient, A.Fake<IAppConfiguration>());
            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();

            // Act
            Func<ActionResult> testCode = () => controller.ChooseActivity(viewModel);

            // Assert
            NotSupportedException error = Assert.Throws<NotSupportedException>(testCode);
            Assert.True(ValidateModel(viewModel).Any(v => v.MemberNames.Contains("SelectedValue") && v.ErrorMessage.Contains("Select the activity you would like to do")));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            // could not get the ModelState.IsValid to false by merely getting the required attribute
            // so this is a little helper
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);

            return validationResults;
        }

        [Fact]
        public void PostChooseActivity_EnableDataReturnsIsFalse_RedirectToSubmissionsHistory()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(false);

            HomeController controller = new HomeController(() => apiClient, configuration);

            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedValue = InternalUserActivity.SubmissionsHistory;

            // Act
            ActionResult result = controller.ChooseActivity(viewModel);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("SubmissionsHistory", redirectToRouteResult.RouteValues["action"]);
        }

        private HomeController HomeController()
        {
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            return new HomeController(() => apiClient, configuration);
        }
    }
}
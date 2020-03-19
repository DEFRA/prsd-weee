﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mediator;
    using Services;
    using System;
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
            var controller = HomeController();
            var result = controller.ChooseActivity();
            var viewResult = ((ViewResult)result);
            Assert.Equal("ChooseActivity", viewResult.ViewName);
        }

        [Fact]
        public void HttpGet_ChooseActivity_ViewModelPossibleValuesShouldBeInCorrectOrder()
        {
            var controller = HomeController();
            var result = controller.ChooseActivity() as ViewResult;
            var model = result.Model as RadioButtonStringCollectionViewModel;

            model.PossibleValues[0].Should().Be(InternalUserActivity.ManageScheme);
            model.PossibleValues[1].Should().Be(InternalUserActivity.SubmissionsHistory);
            model.PossibleValues[2].Should().Be(InternalUserActivity.ProducerDetails);
            model.PossibleValues[3].Should().Be(InternalUserActivity.ManagePcsCharges);
            model.PossibleValues[4].Should().Be(InternalUserActivity.ManageAatfs);
            model.PossibleValues[5].Should().Be(InternalUserActivity.ManageAes);
            model.PossibleValues[6].Should().Be(InternalUserActivity.ManageUsers);
            model.PossibleValues[7].Should().Be(InternalUserActivity.ViewReports);
        }

        [Fact]
        public void HttpPost_ChooseActivity_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = controller.ChooseActivity(A.Dummy<RadioButtonStringCollectionViewModel>());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData(InternalUserActivity.ManageUsers, "Index")]
        [InlineData(InternalUserActivity.ManageScheme, "ManageSchemes")]
        [InlineData(InternalUserActivity.ManageAatfs, "ManageAatfs")]
        [InlineData(InternalUserActivity.ManageAes, "ManageAatfs")]
        [InlineData(InternalUserActivity.ProducerDetails, "Search")]
        [InlineData(InternalUserActivity.SubmissionsHistory, "ChooseSubmissionType")]
        [InlineData(InternalUserActivity.ViewReports, "ChooseReport")]
        [InlineData(InternalUserActivity.ManagePcsCharges, "SelectAuthority")]
        public void HttpPost_ChooseActivity_RedirectsToCorrectControllerAction(string selection, string action)
        {
            // Arrange
            RadioButtonStringCollectionViewModel viewModel = new RadioButtonStringCollectionViewModel();
            viewModel.SelectedValue = selection;

            // Act
            ActionResult result = HomeController().ChooseActivity(viewModel);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);

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

            RadioButtonStringCollectionViewModel viewModel = new RadioButtonStringCollectionViewModel();
            viewModel.SelectedValue = InternalUserActivity.ManagePcsCharges.ToString();

            // Act
            Func<ActionResult> testCode = () => controller.ChooseActivity(viewModel);

            // Assert
            Assert.Throws<InvalidOperationException>(testCode);
        }

        [Fact]
        public void PostChooseActivity_EnableDataReturnsIsFalse_RedirectToSubmissionsHistory()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(false);

            HomeController controller = new HomeController(() => apiClient, configuration);

            RadioButtonStringCollectionViewModel viewModel = new RadioButtonStringCollectionViewModel();
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
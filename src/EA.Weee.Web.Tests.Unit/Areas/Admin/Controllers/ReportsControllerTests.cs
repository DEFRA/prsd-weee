﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Home;
    using Web.Areas.Admin.ViewModels.Reports;
    using Xunit;

    public class ReportsControllerTests
    {
        private readonly IWeeeClient apiClient;

        public ReportsControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
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

            var result = await ReportsController().Index();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Account", routeValues["controller"]);
            Assert.Equal(userStatus, routeValues["userStatus"]);
        }

        [Fact]
        public async void HttpGet_Index_IfUserIsActive_ShouldRedirectToChooseReport()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<IRequest<UserStatus>>._))
                .Returns(UserStatus.Active);

            var result = await ReportsController().Index();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseReport", routeValues["action"]);
            Assert.Equal("Reports", routeValues["controller"]);
        }

        [Fact]
        public void HttpGet_ChooseReport_ShouldReturnsChooseReportView()
        {
            var controller = ReportsController();
            var result = controller.ChooseReport();
            var viewResult = ((ViewResult)result);
            Assert.Equal("ChooseReport", viewResult.ViewName);
        }

        [Fact]
        public void HttpPost_ChooseReport_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = ReportsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = controller.ChooseReport(new ChooseReportViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData(Reports.ProducerDetails, "ProducerDetails")]
        public void HttpPost_ChooseActivity_RedirectsToCorrectControllerAction(string selection, string action)
        {
            // Arrange
            ChooseReportViewModel model = new ChooseReportViewModel { SelectedValue = selection };

            // Act
            ActionResult result = ReportsController().ChooseReport(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);
        }

        private ReportsController ReportsController()
        {
            return new ReportsController(() => apiClient);
        }
    }
}

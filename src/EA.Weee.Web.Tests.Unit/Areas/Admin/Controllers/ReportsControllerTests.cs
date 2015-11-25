namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Home;
    using Web.Areas.Admin.ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;
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
        [InlineData(Reports.PCSCharges, "PCSCharges")]
        [InlineData(Reports.Producerpublicregister, "ProducerPublicRegister")]
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

        [Fact]
        public async void HttpGet_ProducerDetails_ShouldReturnsProducerDetailsView()
        {
            var controller = ReportsController();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAllComplianceYears>._))
                .Returns(new List<int> { 2015, 2016 });

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAllApprovedSchemes>._))
                .Returns(new List<SchemeData> { new SchemeData() });

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._))
                .Returns(new List<UKCompetentAuthorityData> { new UKCompetentAuthorityData() });

            var result = await controller.ProducerDetails();

            var viewResult = ((ViewResult)result);
            Assert.Equal("ProducerDetails", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_ProducerDetails_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = ReportsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.ProducerDetails(new ReportsFilterViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void HttpGet_PCSCharges_ShouldReturnsPCSChargesView()
        {
            var controller = ReportsController();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAllComplianceYears>._))
                .Returns(new List<int> { 2015, 2016 });

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAllApprovedSchemes>._))
                .Returns(new List<SchemeData> { new SchemeData() });

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._))
                .Returns(new List<UKCompetentAuthorityData> { new UKCompetentAuthorityData() });

            var result = await controller.PCSCharges();

            var viewResult = ((ViewResult)result);
            Assert.Equal("PCSCharges", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_PCSCharges_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = ReportsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.PCSCharges(new ReportsFilterViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void HttpGet_ProducerPublicRegister_ShouldReturnsProducerPublilcRegisterView()
        {
            var controller = ReportsController();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAllComplianceYears>._))
                .Returns(new List<int> { 2015, 2016 });
            
            var result = await controller.ProducerPublicRegister();

            var viewResult = ((ViewResult)result);
            Assert.Equal("ProducerPublicRegister", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_ProducerPublicRegister_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = ReportsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.ProducerPublicRegister(new ProducerPublicRegisterViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        private ReportsController ReportsController()
        {
            return new ReportsController(() => apiClient, A.Dummy<BreadcrumbService>());
        }
    }
}

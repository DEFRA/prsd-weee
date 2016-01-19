namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.Reports;
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
        [InlineData(Reports.Producerpublicregister, "ProducerPublicRegister")]
        [InlineData(Reports.ProducerEEEData, "ProducerEEEData")]
        [InlineData(Reports.SchemeWeeeData, "SchemeWeeeData")]
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

        [Fact]
        public async void HttpGet_ProducerEEEData_ShouldReturnsProducerEEEDataView()
        {
            // Arrange
            List<int> years = new List<int>() { 2015, 2016 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAllComplianceYears>._)).Returns(years);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.ProducerEEEData();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "ProducerEEEData");

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2015", y1.Text),
                y2 => Assert.Equal("2016", y2.Text));
        }

        [Fact]
        public async void HttpPost_ProducerEEEData_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = ReportsController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.ProducerEEEData(new ProducersDataViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action calls the API to retrieve
        /// the list of compliance years and returns the "SchemeWeeeData" view with 
        /// a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            // Arrange
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAllComplianceYears>._)).Returns(years);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "SchemeWeeeData");

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action returns
        /// a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "SchemeWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSchemeWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();
            
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.SchemeWeeeData();

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with an invalid view model
        /// calls the API to retrieve the list of compliance years and returns the "SchemeWeeeData"
        /// view with a ProducerDataViewModel that has be populated with the list of years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithInvalidViewModel_ReturnsSchemeWeeeDataProducerDataViewModel()
        {
            // Arrange
            List<int> years = new List<int>() { 2001, 2002 };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAllComplianceYears>._)).Returns(years);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "SchemeWeeeData");

            ProducersDataViewModel model = viewResult.Model as ProducersDataViewModel;
            Assert.NotNull(model);
            Assert.Collection(model.ComplianceYears,
                y1 => Assert.Equal("2001", y1.Text),
                y2 => Assert.Equal("2002", y2.Text));
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with an invalid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithInvalidViewModel_SetsTriggerDownloadToFalse()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            controller.ModelState.AddModelError("Key", "Error");
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action with a valid view model
        /// returns a view with the ViewBag property "TriggerDownload" set to true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_WithViewModel_SetsTriggerDownloadToTrue()
        {
            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.SchemeWeeeData(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "SchemeWeeeData" action sets
        /// the breadcrumb's internal activity to "View reports".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostSchemeWeeeData_Always_SetsInternalBreadcrumbToViewReports()
        {
            BreadcrumbService breadcrumb = new BreadcrumbService();

            // Arrange
            ReportsController controller = new ReportsController(
                () => A.Dummy<IWeeeClient>(),
                breadcrumb);

            // Act
            ActionResult result = await controller.SchemeWeeeData(A.Dummy<ProducersDataViewModel>());

            // Assert
            Assert.Equal("View reports", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadSchemeWeeeDataCsv" action will
        /// call the API to generate a CSV file which is returned with the correct file name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadSchemeWeeeDataCsv_Always_CallsApiAndReturnsFileResultWithCorrectFileName()
        {
            // Arrange
            FileInfo file = new FileInfo("TEST FILE.csv", new byte[] { 1, 2, 3 });

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeWeeeCsv>._))
                .WhenArgumentsMatch(a => 
                    a.Get<GetSchemeWeeeCsv>("request").ComplianceYear == 2015 &&
                    a.Get<GetSchemeWeeeCsv>("request").ObligationType == ObligationType.B2C)
                .Returns(file);

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadSchemeWeeeDataCsv(2015, ObligationType.B2C);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("TEST FILE.csv", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadSchemeWeeeDataCsv" action will
        /// call the API to generate a CSV file which is returned with a content
        /// type of "text/plain"
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadSchemeWeeeDataCsv_Always_CallsApiAndReturnsFileResultWithContentTypeOfTextPlain()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<GetSchemeWeeeCsv>._))
                .WhenArgumentsMatch(a => a.Get<int>("complianceYear") == 2015 && a.Get<ObligationType>("obligationType") == ObligationType.B2C)
                .Returns(A.Dummy<FileInfo>());

            ReportsController controller = new ReportsController(
                () => weeeClient,
                A.Dummy<BreadcrumbService>());

            ProducersDataViewModel viewModel = new ProducersDataViewModel();

            // Act
            ActionResult result = await controller.DownloadSchemeWeeeDataCsv(2015, ObligationType.B2C);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/plain", fileResult.ContentType);
        }

        private ReportsController ReportsController()
        {
            return new ReportsController(() => apiClient, A.Dummy<BreadcrumbService>());
        }
    }
}

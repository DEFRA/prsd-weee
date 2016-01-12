namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Admin;
    using Core.Charges;
    using Core.Shared;
    using FakeItEasy;
    using Services;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Charge;
    using Weee.Requests.Charges;
    using Xunit;

    public class ChargeControllerTests
    {
        /// <summary>
        /// This test ensures that the OnActionExecuting method will throw an
        /// InvalidOperationException if the application configuration has "EnableInvoicing"
        /// set to false.
        /// </summary>
        [Fact]
        public void OnActionExecuting_WhenInvoicingDisabledInConfig_ThrowsInvalidOperationException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableInvoicing).Returns(false);

            ChargeController controller = new ChargeController(
                configuration,
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            MethodInfo onActionExecutingMethod = typeof(ChargeController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            Assert.Throws<InvalidOperationException>(testCode);
        }

        /// <summary>
        /// This test ensures that the OnActionExecuting method will not throw an
        /// exception if the application configuration has "EnableInvoicing"
        /// set to true.
        /// </summary>
        [Fact]
        public void OnActionExecuting_WhenInvoicingEnabledInConfig_DoesntThrowException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);

            ChargeController controller = new ChargeController(
                configuration,
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            MethodInfo onActionExecutingMethod = typeof(ChargeController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
            try
            {
                onActionExecutingMethod.Invoke(controller, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            // Assert
            // No exception.
        }

        /// <summary>
        /// This test ensures that the OnActionExecuting method will set the InternalActivity
        /// property of the breadcrumb to "Manage PCS charges".
        /// </summary>
        [Fact]
        public void OnActionExecuting_Always_SetsBreadcrumbInternalActivityToManageCharges()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableInvoicing).Returns(true);

            BreadcrumbService breadcrumb = new BreadcrumbService();

            ChargeController controller = new ChargeController(
                configuration,
                breadcrumb,
                () => A.Dummy<IWeeeClient>());

            // Act
            MethodInfo onActionExecutingMethod = typeof(ChargeController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
            try
            {
                onActionExecutingMethod.Invoke(controller, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            // Assert
            Assert.Equal("Manage PCS charges", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "SelectAuthority" action will return the
        /// "SelectAuthority" view with a view model.
        /// </summary>
        [Fact]
        public void GetSelectAuthority_Always_ReturnsSelectAuthorityViewWithViewModel()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = controller.SelectAuthority();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "SelectAuthority");

            SelectAuthorityViewModel viewModel = viewResult.Model as SelectAuthorityViewModel;
            Assert.NotNull(viewModel);
        }

        /// <summary>
        /// This test ensures that the POST "SelectAuthority" action will return the
        /// "SelectAuthority" view a view model if the view model provided is invalid.
        /// </summary>
        [Fact]
        public void PostSelectAuthority_WithInvalidModel_ReturnsSelectAuthorityViewWithViewModel()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            controller.ModelState.AddModelError("key", "Some error");

            // Act
            ActionResult result = controller.SelectAuthority(A.Dummy<SelectAuthorityViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "SelectAuthority");

            SelectAuthorityViewModel viewModel = viewResult.Model as SelectAuthorityViewModel;
            Assert.NotNull(viewModel);
        }

        /// <summary>
        /// This test ensures that the POST "SelectAuthority" action will return a redirect
        /// to the "ChooseActivity" action, with the selected authority.
        /// </summary>
        [Fact]
        public void PostSelectAuthority_WithValidModel_RedirectsToChooseActivityActionWithSelectedAuthority()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            SelectAuthorityViewModel viewModel = new SelectAuthorityViewModel();
            viewModel.SelectedAuthority = CompetentAuthority.NorthernIreland;

            // Act
            ActionResult result = controller.SelectAuthority(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ChooseActivity", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
        }

        /// <summary>
        /// This test ensures that the GET "ChooseActivity" action will return the
        /// "ChooseActivity" view with a view model and adds the specified authority
        /// to the ViewBag.
        /// </summary>
        [Fact]
        public void GetChooseActivity_Always_ReturnsChooseActivityViewWithViewModelAndAddsAuthorityToViewBag()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = controller.ChooseActivity(CompetentAuthority.NorthernIreland);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "ChooseActivity");

            ChooseActivityViewModel viewModel = viewResult.Model as ChooseActivityViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(CompetentAuthority.NorthernIreland, (object)viewResult.ViewBag.Authority);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseActivity" action will return the
        /// "ChooseActivity" view a view model and adds the specified authority to 
        /// the ViewBag if the view model provided is invalid.
        /// </summary>
        [Fact]
        public void PostChooseActivity_WithInvalidModel_ReturnsChooseActivityViewWithViewModelAndAddsAuthorityToViewBag()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            controller.ModelState.AddModelError("key", "Some error");

            // Act
            ActionResult result = controller.ChooseActivity(CompetentAuthority.NorthernIreland, A.Dummy<ChooseActivityViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "ChooseActivity");

            ChooseActivityViewModel viewModel = viewResult.Model as ChooseActivityViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(CompetentAuthority.NorthernIreland, (object)viewResult.ViewBag.Authority);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseActivity" action will return a redirect
        /// to the "ManagePendingCharges" action, with the selected authority when option
        /// to manage pending charges is selected.
        /// </summary>
        [Fact]
        public void PostChooseActivity_WithManagePendingChargesSelected_RedirectsToManagePendingChargesActionWithSelectedAuthority()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedActivity = Activity.ManagePendingCharges;

            // Act
            ActionResult result = controller.ChooseActivity(CompetentAuthority.NorthernIreland, viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ManagePendingCharges", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseActivity" action will return a redirect
        /// to the "IssuedCharges" action, with the selected authority when option
        /// to manage issued charges is selected.
        /// </summary>
        [Fact]
        public void PostChooseActivity_WithManageIssuedChargesSelected_RedirectsToIssuedChargesActionWithSelectedAuthority()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedActivity = Activity.ManageIssuedCharges;

            // Act
            ActionResult result = controller.ChooseActivity(CompetentAuthority.NorthernIreland, viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("IssuedCharges", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
        }

        /// <summary>
        /// This test ensures that the POST "ChooseActivity" action will return a redirect
        /// to the "InvoiceRuns" action, with the selected authority when option
        /// to view invoice run history is selected.
        /// </summary>
        [Fact]
        public void PostChooseActivity_WithViewInvoiceRunHistorySelected_RedirectsToInvoiceRunsActionWithSelectedAuthority()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            viewModel.SelectedActivity = Activity.ViewInvoiceRunHistory;

            // Act
            ActionResult result = controller.ChooseActivity(CompetentAuthority.NorthernIreland, viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("InvoiceRuns", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
        }

        /// <summary>
        /// This test ensures that the GET "ManagePendingCharges" action will return the
        /// "ManagePendingCharges" view with a view model containing data retrieved from the API
        /// and adds the specified authority to the ViewBag.
        /// </summary>
        [Fact]
        public async Task GetManagePendingCharges_Always_ReturnsManagePendingChargesViewWithViewModelAndAddsAuthorityToViewBag()
        {
            IList<PendingCharge> pendingCharges = A.Dummy<IList<PendingCharge>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchPendingCharges>._))
                .Returns(pendingCharges);

            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.ManagePendingCharges(CompetentAuthority.NorthernIreland);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "ManagePendingCharges");

            IList<PendingCharge> viewModel = viewResult.Model as IList<PendingCharge>;
            Assert.Equal(pendingCharges, viewModel);

            Assert.Equal(CompetentAuthority.NorthernIreland, (object)viewResult.ViewBag.Authority);
        }

        /// <summary>
        /// This test ensures that the POST "ManagePendingCharges" action will call the API to issue charges
        /// amd then return a redirect to the "ChargesSuccessfullyIssued" action with the selected authority and the ID of
        /// the invoice run returned by the API.
        /// </summary>
        [Fact]
        public async Task PostManagePendingCharges_Always_CallsApiAndRedirectsToChargesSuccessfullyIssuedActionWithAuthorityAndInvoiceRunId()
        {
            Guid invoiceRunId = new Guid("FB95F6E7-8809-488A-B23B-5B3F5A9B3D5F");

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IssuePendingCharges>._))
                .Returns(invoiceRunId);

            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.ManagePendingCharges(CompetentAuthority.NorthernIreland, A.Dummy<FormCollection>());

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ChargesSuccessfullyIssued", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
            Assert.Equal(invoiceRunId, redirectResult.RouteValues["id"]);
        }

        /// <summary>
        /// This test ensures that the GET "ChargesSucessfullyIssued" action will return the "ChargesSuccessfullyIssued" view
        /// providing the invoice run ID as the model. Where the authority is the Environment Agency, the ViewBag
        /// will have AllowDownloadOfInvoiceFiles set to true.
        /// </summary>
        [Fact]
        public void GetChargesSuccessfullyIssued_ForEngland_ReturnsChargesSuccessfullyIssuedViewWithModelAllowingInvoiceDownload()
        {
            // Arrange
            Guid invoiceRunId = new Guid("23610E5E-5B79-4DB0-BD8C-0213A9B44C45");

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = controller.ChargesSuccessfullyIssued(CompetentAuthority.England, invoiceRunId);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "ChargesSuccessfullyIssued");

            Guid? viewModel = viewResult.Model as Guid?;
            Assert.NotNull(viewModel);
            Assert.Equal(invoiceRunId, viewModel);
            Assert.Equal(true, viewResult.ViewBag.AllowDownloadOfInvoiceFiles);
        }

        /// <summary>
        /// This test ensures that the GET "ChargesSucessfullyIssued" action will return the "ChargesSuccessfullyIssued" view
        /// providing the invoice run ID as the model. Where the authority is devloved, the ViewBag
        /// will have AllowDownloadOfInvoiceFiles set to false.
        /// </summary>
        [Fact]
        public void GetChargesSuccessfullyIssued_ForDevolvedAuthority_ReturnsChargesSuccessfullyIssuedViewWithModelNotAllowingInvoiceDownload()
        {
            // Arrange
            Guid invoiceRunId = new Guid("23610E5E-5B79-4DB0-BD8C-0213A9B44C45");

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = controller.ChargesSuccessfullyIssued(CompetentAuthority.Scotland, invoiceRunId);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "ChargesSuccessfullyIssued");

            Guid? viewModel = viewResult.Model as Guid?;
            Assert.NotNull(viewModel);
            Assert.Equal(invoiceRunId, viewModel);
            Assert.Equal(false, viewResult.ViewBag.AllowDownloadOfInvoiceFiles);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadInvoiceFiles" action for England will call the API to retrieve
        /// data for a ZIP file representing the specified invoice run's 1B1S files; and that this data will
        /// be returned as a FileResult with the correct file name and a content type of "text/plain".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadInvoiceFiles_ForEngland_CallsApiAndReturnsFileResult()
        {
            // Arrange
            Guid invoiceRunId = new Guid("ADED8BDE-CF03-4696-B972-DDAB9306A6DD");

            FileInfo fileInfo = new FileInfo("Test file.zip", A.Dummy<byte[]>());

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchInvoiceRunIbisZipFile>._))
                .WhenArgumentsMatch(a => a.Get<FetchInvoiceRunIbisZipFile>("request").InvoiceRunId == invoiceRunId)
                .Returns(fileInfo);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.DownloadInvoiceFiles(
                CompetentAuthority.England,
                invoiceRunId);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("Test file.zip", fileResult.FileDownloadName);
            Assert.Equal("text/plain", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadInvoiceFiles" action for a devloved authority will throw
        /// an InvalidOperationException.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadInvoiceFiles_ForDevolvedAuthority_ThowsInvalidOperationException()
        {
            // Arrange
            Guid invoiceRunId = new Guid("ADED8BDE-CF03-4696-B972-DDAB9306A6DD");

            FileInfo fileInfo = new FileInfo("Test file.zip", A.Dummy<byte[]>());

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchInvoiceRunIbisZipFile>._))
                .WhenArgumentsMatch(a => a.Get<FetchInvoiceRunIbisZipFile>("request").InvoiceRunId == invoiceRunId)
                .Returns(fileInfo);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            Func<Task<ActionResult>> testCode = async () => await controller.DownloadInvoiceFiles(
                CompetentAuthority.Scotland,
                invoiceRunId);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        }

        /// <summary>
        /// This test ensures that the GET "InvoiceRuns" action will return the "InvoiceRuns" view
        /// providing the list of invoice runs. Where the authority is the Environment Agency, the ViewBag
        /// will have AllowDownloadOfInvoiceFiles set to true.
        /// </summary>
        [Fact]
        public async Task GetInvoiceRuns_ForEngland_ReturnsInvoiceRunsViewWithModelAllowingInvoiceDownload()
        {
            // Arrange
            IReadOnlyList<InvoiceRunInfo> results = A.Dummy<IReadOnlyList<InvoiceRunInfo>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<FetchInvoiceRuns>._)).Returns(results);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.InvoiceRuns(CompetentAuthority.England);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "InvoiceRuns");

            IReadOnlyList<InvoiceRunInfo> viewModel = viewResult.Model as IReadOnlyList<InvoiceRunInfo>;
            Assert.NotNull(viewModel);
            Assert.Equal(results, viewModel);
            Assert.Equal(true, viewResult.ViewBag.AllowDownloadOfInvoiceFiles);
        }

        /// <summary>
        /// This test ensures that the GET "InvoiceRuns" action will return the "InvoiceRuns" view
        /// providing the list of invoice runs. Where the authority is devolved, the ViewBag
        /// will have AllowDownloadOfInvoiceFiles set to false.
        /// </summary>
        [Fact]
        public async Task GetInvoiceRuns_ForDevolvedAuthority_ReturnsInvoiceRunsViewWithModelNotAllowingInvoiceDownload()
        {
            // Arrange
            IReadOnlyList<InvoiceRunInfo> results = A.Dummy<IReadOnlyList<InvoiceRunInfo>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<FetchInvoiceRuns>._)).Returns(results);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.InvoiceRuns(CompetentAuthority.Scotland);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "InvoiceRuns");

            IReadOnlyList<InvoiceRunInfo> viewModel = viewResult.Model as IReadOnlyList<InvoiceRunInfo>;
            Assert.NotNull(viewModel);
            Assert.Equal(results, viewModel);
            Assert.Equal(false, viewResult.ViewBag.AllowDownloadOfInvoiceFiles);
        }

        [Fact]
        public async Task GetDownloadChargeBreakdown_CallsApiAndReturnsFileResult()
        {
            // Arrange
            Guid invoiceRunId = Guid.NewGuid();

            var csvFileData = new CSVFileData { FileName = "Test file.csv", FileContent = "CSV content" };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchInvoiceRunCsv>._))
                .WhenArgumentsMatch(a => a.Get<FetchInvoiceRunCsv>("request").InvoiceRunId == invoiceRunId)
                .Returns(csvFileData);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.DownloadChargeBreakdown(invoiceRunId);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("Test file.csv", fileResult.FileDownloadName);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        /// <summary>
        /// This test ensures that the GET "IssuedCharges" action will return the "IssuedCharges" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIssuedCharges_Always_ReturnsIssuedChargesView()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(A.Dummy<CompetentAuthority>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "IssuedCharges");
        }

        /// <summary>
        /// This test ensures that the GET "IssuedCharges" action will call the API
        /// to fetch a list of compliance years and scheme names which will be populated
        /// into the view model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIssuedCharges_Always_CallsApiAndPopulatesViewModel()
        {
            // Arrange
            CompetentAuthority authority = A.Dummy<CompetentAuthority>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> complianceYears = A.Dummy<List<int>>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchComplianceYearsWithInvoices>._))
                .WhenArgumentsMatch(a => a.Get<FetchComplianceYearsWithInvoices>("request").Authority == authority)
                .Returns(complianceYears);

            List<string> schemeNames = A.Dummy<List<string>>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchSchemesWithInvoices>._))
                .WhenArgumentsMatch(a => a.Get<FetchSchemesWithInvoices>("request").Authority == authority)
                .Returns(schemeNames);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.IssuedCharges(authority);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            IssuedChargesViewModel viewModel = viewResult.Model as IssuedChargesViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(complianceYears, viewModel.ComplianceYears);
            Assert.Equal(schemeNames, viewModel.SchemeNames);
        }

        /// <summary>
        /// This test ensures that the GET "IssuedCharges" action will populate
        /// the "Authority" property of the ViewBag with the specified authority.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIssuedCharges_Always_PopulatesViewBagWithAuthority()
        {
            // Arrange
            CompetentAuthority authority = A.Dummy<CompetentAuthority>();

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(authority);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(authority, viewResult.ViewBag.Authority);
        }

        /// <summary>
        /// This test ensures that the GET "IssuedCharges" action will populate
        /// the "TriggerDownload" property of the ViewBag with false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIssuedCharges_Always_PopulatesViewBagTriggerDownloadWithFalse()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(A.Dummy<CompetentAuthority>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "IssuedCharges" action will return the "IssuedCharges" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostIssuedCharges_Always_ReturnsIssuedChargesView()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(
                A.Dummy<CompetentAuthority>(),
                A.Dummy<IssuedChargesViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "IssuedCharges");
        }

        /// <summary>
        /// This test ensures that the POST "IssuedCharges" action will call the API
        /// to fetch a list of compliance years and scheme names which will be populated
        /// into the view model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostIssuedCharges_Always_CallsApiAndPopulatesViewModel()
        {
            // Arrange
            CompetentAuthority authority = A.Dummy<CompetentAuthority>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            List<int> complianceYears = A.Dummy<List<int>>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchComplianceYearsWithInvoices>._))
                .WhenArgumentsMatch(a => a.Get<FetchComplianceYearsWithInvoices>("request").Authority == authority)
                .Returns(complianceYears);

            List<string> schemeNames = A.Dummy<List<string>>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchSchemesWithInvoices>._))
                .WhenArgumentsMatch(a => a.Get<FetchSchemesWithInvoices>("request").Authority == authority)
                .Returns(schemeNames);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.IssuedCharges(authority, A.Dummy<IssuedChargesViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            IssuedChargesViewModel viewModel = viewResult.Model as IssuedChargesViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(complianceYears, viewModel.ComplianceYears);
            Assert.Equal(schemeNames, viewModel.SchemeNames);
        }

        /// <summary>
        /// This test ensures that the POST "IssuedCharges" action will populate
        /// the "Authority" property of the ViewBag with the specified authority.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostIssuedCharges_Always_PopulatesViewBagWithAuthority()
        {
            // Arrange
            CompetentAuthority authority = A.Dummy<CompetentAuthority>();

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(authority, A.Dummy<IssuedChargesViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(authority, viewResult.ViewBag.Authority);
        }

        /// <summary>
        /// This test ensures that the POST "IssuedCharges" action will populate
        /// the "TriggerDownload" property of the ViewBag with false if there are
        /// validation errors.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostIssuedCharges_WithValidationErrors_PopulatesViewBagTriggerDownloadWithFalse()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            controller.ModelState.AddModelError("key", "error");

            // Act
            ActionResult result = await controller.IssuedCharges(A.Dummy<CompetentAuthority>(), A.Dummy<IssuedChargesViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(false, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the POST "IssuedCharges" action will populate
        /// the "TriggerDownload" property of the ViewBag with true if there are
        /// no validation errors.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostIssuedCharges_WithNoValidationErrors_PopulatesViewBagTriggerDownloadWithTrue()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.IssuedCharges(A.Dummy<CompetentAuthority>(), A.Dummy<IssuedChargesViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal(true, viewResult.ViewBag.TriggerDownload);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadIssuedChargesCsv" action will always return
        /// a file result with a file name provided by calling the API.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadIssuedChargesCsv_Always_CallsApiAndReturnsFileResultWithCorrectFileName()
        {
            // Arrange
            CompetentAuthority authority = A.Dummy<CompetentAuthority>();
            int complianceYear = A.Dummy<int>();
            string schemeName = A.Dummy<string>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            FileInfo fileInfo = new FileInfo("filename", new byte[] { 1, 2, 3 });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchIssuedChargesCsv>._))
                .WhenArgumentsMatch(a => a.Get<FetchIssuedChargesCsv>("request").Authority == authority
                    && a.Get<FetchIssuedChargesCsv>("request").ComplianceYear == complianceYear
                    && a.Get<FetchIssuedChargesCsv>("request").SchemeName == schemeName)
                .Returns(fileInfo);

            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => weeeClient);

            // Act
            ActionResult result = await controller.DownloadIssuedChargesCsv(authority, complianceYear, schemeName);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("filename", fileResult.FileDownloadName);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadIssuedChargesCsv" action will always
        /// return a file with a content type of "text/plain".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadIssuedChargesCsv_Always_ReturnsFileResultWithContentTypeOfTextPlain()
        {
            // Arrange
            ChargeController controller = new ChargeController(
                A.Dummy<IAppConfiguration>(),
                A.Dummy<BreadcrumbService>(),
                () => A.Dummy<IWeeeClient>());

            // Act
            ActionResult result = await controller.DownloadIssuedChargesCsv(
                A.Dummy<CompetentAuthority>(),
                A.Dummy<int>(),
                A.Dummy<string>());

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/plain", fileResult.ContentType);
        }
    }
}

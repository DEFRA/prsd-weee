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
        /// property of the breadcrumb to "Manage charges".
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
            Assert.Equal("Manage charges", breadcrumb.InternalActivity);
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
        /// amd then return a redirect to the "InvoiceRun" action with the selected authority and the ID of
        /// the invoice run returned by the API.
        /// </summary>
        [Fact]
        public async Task PostManagePendingCharges_Always_CallsApiAndRedirectsToInvoiceRunActionWithAuthorityAndInvoiceRunId()
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

            Assert.Equal("InvoiceRun", redirectResult.RouteValues["action"]);
            Assert.Equal(CompetentAuthority.NorthernIreland, redirectResult.RouteValues["authority"]);
            Assert.Equal(invoiceRunId, redirectResult.RouteValues["id"]);
        }
    }
}

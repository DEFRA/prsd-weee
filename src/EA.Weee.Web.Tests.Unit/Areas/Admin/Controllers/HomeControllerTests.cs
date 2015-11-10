namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Home;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient apiClient;

        public HomeControllerTests()
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
        public void HttpPost_ChooseActivity_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = controller.ChooseActivity(new InternalUserActivityViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData(InternalUserActivity.ManageUsers, "ManageUsers")]
        [InlineData(InternalUserActivity.ManageScheme, "ManageSchemes")]
        [InlineData(InternalUserActivity.ViewProducerInformation, "Search")]
        [InlineData(InternalUserActivity.SubmissionsHistory, "SubmissionsHistory")]
        [InlineData(InternalUserActivity.ViewReports, "ChooseReport")]
        public void HttpPost_ChooseActivity_RedirectsToCorrectControllerAction(string selection, string action)
        {
            // Arrange
            InternalUserActivityViewModel model = new InternalUserActivityViewModel { SelectedValue = selection };

            // Act
            ActionResult result = HomeController().ChooseActivity(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal(action, redirectToRouteResult.RouteValues["action"]);
        }

        private HomeController HomeController()
        {
            return new HomeController(() => apiClient);
        }
    }
}
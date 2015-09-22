namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using FakeItEasy;
    using TestHelpers;
    using ViewModels.Shared;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Weee.Requests.Admin;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;
     
        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }
        [Fact]
        public void HttpGet_ChooseActivity_ShouldReturnsChooseActivityView()
        {
           var result = HomeController().ChooseActivity();
            var viewResult = ((ViewResult)result);
            Assert.Equal("ChooseActivity", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_ChooseActivity_ModelIsInvalid_ShouldRedirectViewWithError()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var result = await controller.ChooseActivity(new InternalUserActivityViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void PostChooseActivity_ManageSchemesPendingStatus_RedirectsToAuthorisationRequired()
        {
           A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Pending);
            var result = await HomeController().ChooseActivity(new InternalUserActivityViewModel
            {
                InternalUserActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = InternalUserActivity.ManageScheme
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManageSchemesRejectedStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Rejected);

            var result = await HomeController().ChooseActivity(new InternalUserActivityViewModel
            {
                InternalUserActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = InternalUserActivity.ManageScheme
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManageUsersPendingStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Pending);

            var result = await HomeController().ChooseActivity(new InternalUserActivityViewModel
            {
                InternalUserActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = InternalUserActivity.ManageUsers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManageUsersRejectedStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Rejected);

            var result = await HomeController().ChooseActivity(new InternalUserActivityViewModel
            {
                InternalUserActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = InternalUserActivity.ManageUsers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }
        private HomeController HomeController()
        {
            var controller = new HomeController(() => weeeClient);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }
    }
}

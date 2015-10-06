namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core;
    using FakeItEasy;
    using Services;
    using Services.Caching;
    using Web.Controllers;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeCache cache;

        public HomeControllerTests()
        {
            cache = A.Fake<IWeeeCache>();
        }

        [Fact]
        public void HttpGet_Index_UserIsAuthenticated_AndInternal_RedirectsToAdminHomeIndex()
        {
            var controller = HomeController(true, Claims.CanAccessInternalArea);

            var result = controller.Index();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Admin", routeValues["area"]);
            Assert.Equal("Home", routeValues["controller"]);
            Assert.Equal("Index", routeValues["action"]);
        }

        [Fact]
        public void HttpGet_Index_UserIsAuthenticated_AndExternal_RedirectsToSelectOrganisationRoute()
        {
            var controller = HomeController(true, Claims.CanAccessExternalArea);

            var result = controller.Index();

            Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("SelectOrganisation", ((RedirectToRouteResult)result).RouteName);
        }

        private HomeController HomeController(bool userIsAuthenticated, params string[] authenticationMethodClaims)
        {
            var claimsIdentity = A.Fake<ClaimsIdentity>();
            A.CallTo(() => claimsIdentity.IsAuthenticated)
                .Returns(userIsAuthenticated);

            A.CallTo(() => claimsIdentity.HasClaim(A<string>._, A<string>._))
                .ReturnsLazily(
                    (string type, string value) =>
                        type == ClaimTypes.AuthenticationMethod && authenticationMethodClaims.Contains(value));

            var userPrincipal = A.Fake<IPrincipal>();

            A.CallTo(() => userPrincipal.Identity)
                .Returns(claimsIdentity);

            var httpContext = A.Fake<HttpContextBase>();
            httpContext.User = userPrincipal;

            var requestContext = new RequestContext(httpContext, new RouteData());

            var controller = new HomeController(new BreadcrumbService(), cache);
            controller.ControllerContext = new ControllerContext(requestContext, controller);

            return controller;
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Infrastructure
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using EA.Weee.Web.Areas.Admin;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SessionTimeoutRoutingTests
    {
        [Fact]
        public void GenerateVirtualPath_ForAreaLessRoute_DoesNotThrow_WhenNoAreaRequested()
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var httpContext = CreateHttpContextForAreaLessRequest();
            var routeData = routes.GetRouteData(httpContext);
            var requestContext = new RequestContext(httpContext, routeData);

            Action act = () => routes.GetVirtualPath(requestContext, new RouteValueDictionary(
                new { action = "SignOut", controller = "Account", area = string.Empty }));

            act.Should().NotThrow();
        }

        [Fact]
        public void GenerateVirtualPath_ForAreaLessRoute_Throws_WhenAdminAreaRequested()
        {
            // This documents the known framework limitation that caused the production bug:
            // generating a URL for a different area when ambient route data has
            // no area key at all throws ArgumentNullException in classic routing.
            var routes = new RouteCollection();
            RegisterAdminAreaRoutes(routes);
            RouteConfig.RegisterRoutes(routes);

            var httpContext = CreateHttpContextForAreaLessRequest();
            var routeData = routes.GetRouteData(httpContext);
            var requestContext = new RequestContext(httpContext, routeData);

            Action act = () => routes.GetVirtualPath(requestContext, new RouteValueDictionary(
                new { action = "SignOut", controller = "Account", area = "admin" }));

            act.Should().Throw<ArgumentNullException>();
        }

        private static void RegisterAdminAreaRoutes(RouteCollection routes)
        {
            var context = new AreaRegistrationContext("Admin", routes);
            new AdminAreaRegistration().RegisterArea(context);
        }

        private static HttpContextBase CreateHttpContextForAreaLessRequest()
        {
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => request.AppRelativeCurrentExecutionFilePath).Returns("~/");
            A.CallTo(() => request.PathInfo).Returns(string.Empty);
            A.CallTo(() => request.HttpMethod).Returns("GET");

            var response = A.Fake<HttpResponseBase>();
            A.CallTo(() => response.ApplyAppPathModifier(A<string>.Ignored))
                .ReturnsLazily(call => (string)call.Arguments[0]);

            var context = A.Fake<HttpContextBase>();
            A.CallTo(() => context.Request).Returns(request);
            A.CallTo(() => context.Response).Returns(response);

            return context;
        }
    }
}
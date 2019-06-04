namespace EA.Weee.Web.Tests.Unit.Infrastructure
{
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Web.Mvc;
    using Xunit;

    public class ARedirectTests
    {
        [Fact]
        public void ReturnsListRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.ReturnsList(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("Index", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }

        [Fact]
        public void ExportedWholeWeeeRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.ExportedWholeWeee(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("ExportedWholeWeee", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }

        [Fact]
        public void NilREturnRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.NilReturn(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("NilReturn", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }
    }
}

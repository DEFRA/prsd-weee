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
        public void NilReturnRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();
            Guid returnId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.NilReturn(orgId, returnId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("NilReturn", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
            Assert.Equal(returnId, newRoute.RouteValues["returnId"]);
        }

        [Fact]
        public void ConfirmationRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();
            Guid returnId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.Confirmation(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("Confirmation", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Infrastructure
{
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Web.Mvc;
    using Core.DataReturns;
    using FakeItEasy;
    using Xunit;

    public class ARedirectTests
    {
        [Fact]
        public void ReturnsListRedirectCalled_ShouldReturnCorrectRoute()
        {
            var orgId = Guid.NewGuid();

            var newRoute = AeRedirect.ReturnsList(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("Index", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }

        [Fact]
        public void ExportedWholeWeeeRedirectCalled_ShouldReturnCorrectRoute()
        {
            Guid orgId = Guid.NewGuid();
            Guid returnId = Guid.NewGuid();

            RedirectToRouteResult newRoute = AeRedirect.ExportedWholeWeee(orgId, returnId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("ExportedWholeWeee", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
            Assert.Equal(returnId, newRoute.RouteValues["returnId"]);
        }

        [Fact]
        public void NilReturnRedirectCalled_ShouldReturnCorrectRoute()
        {
            var orgId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var newRoute = AeRedirect.NilReturn(orgId, returnId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("NilReturn", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
            Assert.Equal(returnId, newRoute.RouteValues["returnId"]);
        }

        [Fact]
        public void ConfirmationRedirectCalled_ShouldReturnCorrectRoute()
        {
            var orgId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var newRoute = AeRedirect.Confirmation(orgId);

            Assert.Equal(AeRedirect.ReturnsRouteName, newRoute.RouteName);
            Assert.Equal("Returns", newRoute.RouteValues["controller"]);
            Assert.Equal("Confirmation", newRoute.RouteValues["action"]);
            Assert.Equal(orgId, newRoute.RouteValues["organisationId"]);
        }
    }
}

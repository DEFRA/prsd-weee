namespace EA.Weee.Web.Tests.Unit.Filters
{
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Filters;
    using Xunit;

    public class MaintenanceModeFilterAttributeTests
    {
        private readonly MaintenanceModeFilterAttribute filter;
        private readonly ActionExecutedContext context;

        public MaintenanceModeFilterAttributeTests()
        {
            context = A.Fake<ActionExecutedContext>();
            filter = new MaintenanceModeFilterAttribute();
        }

        [Fact]
        public void OnActionExecuted_GivenNotMaintenanceAction_ShouldRedirectToMaintenanceAction()
        {
            var routeData = new RouteData();
            routeData.Values.Add("action", "NotMaintenance");
            routeData.Values.Add("controller", "NotError");

            A.CallTo(() => context.RouteData).Returns(routeData);

            filter.OnActionExecuted(context);

            context.RouteData.Values["action"].Should().Be("Maintenance");
            context.RouteData.Values["controller"].Should().Be("Error");
        }

        [Fact]
        public void OnActionExecuted_GivenMaintenanceAction_ShouldRouteDataShouldBeMaintenanceAction()
        {
            var routeData = new RouteData();
            routeData.Values.Add("action", "Maintenance");
            routeData.Values.Add("controller", "Error");

            A.CallTo(() => context.RouteData).Returns(routeData);

            filter.OnActionExecuted(context);

            context.RouteData.Values["action"].Should().Be("Maintenance");
            context.RouteData.Values["controller"].Should().Be("Error");
        }
    }
}

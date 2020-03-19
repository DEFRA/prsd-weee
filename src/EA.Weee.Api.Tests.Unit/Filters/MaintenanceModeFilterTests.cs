namespace EA.Weee.Api.Tests.Unit.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using Api.Filters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class MaintenanceModeFilterTests
    {
        [Fact]
        public async Task MaintenanceModeFilter_OnActionExecutes_ShouldReturnHttpServiceUnavailable()
        {
            var filter = new MaintenanceModeFilter();
            var httpContext = new HttpActionContext();
            var request = new HttpRequestMessage();
            var controllerContext = new HttpControllerContext {Request = request};
            httpContext.ControllerContext = controllerContext;

            var result = await filter.ExecuteActionFilterAsync(httpContext, new CancellationToken(), null);

            result.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Startup
{
    using System.Linq;
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core.Web.Mvc.Filters;
    using Services;
    using Web.Filters;
    using Web.Infrastructure;
    using Xunit;

    public class GlobalFilterConfigurationTests
    {
        [Fact]
        public void GlobalFilters_ShouldContainCorrectFilters()
        {
            GlobalFilters.Filters.Clear();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, A.Fake<IAppConfiguration>());

            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(RequireHttpsAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(HandleApiErrorAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(HandleErrorAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(RenderActionErrorAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(AuthorizeAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(UserAccountActivationAttribute))).Should().Be(1);
            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(AntiForgeryErrorFilter))).Should().Be(1);
        }

        [Fact]
        public void GlobalFilters_GivenMaintenanceMode_FiltersShouldContainMaintenanceModeFilter()
        {
            GlobalFilters.Filters.Clear();

            var configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.MaintenanceMode).Returns(true);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, configuration);

            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(MaintenanceModeFilterAttribute))).Should().Be(1);
        }

        [Fact]
        public void GlobalFilters_GivenNotMaintenanceMode_FiltersShouldNotContainMaintenanceModeFilter()
        {
            GlobalFilters.Filters.Clear();

            var configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.MaintenanceMode).Returns(false);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, configuration);

            GlobalFilters.Filters.Count(x => x.Instance.GetType() == (typeof(MaintenanceModeFilterAttribute))).Should().Be(0);
        }
    }
}

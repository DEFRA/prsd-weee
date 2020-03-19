namespace EA.Weee.Api.Tests.Unit.Startup
{
    using System.Linq;
    using Api.Filters;
    using Elmah.Contrib.WebApi;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Xunit;

    public class FilterConfigTests
    {
        private readonly FilterConfig config;
        private readonly IAppConfiguration configuration;

        public FilterConfigTests()
        {
            configuration = A.Fake<IAppConfiguration>();

            config = new FilterConfig(configuration);
        }

        [Fact]
        public void FilterConfig_GivenConfig_ShouldContainElmahHandleErrorApiAttribute()
        {
            config.Collection.Count(x => x.GetType() == (typeof(ElmahHandleErrorApiAttribute))).Should().Be(1);
        }

        [Fact]
        public void FilterConfig_GivenMaintenanceMode_ShouldContainMaintenanceModeFilter()
        {
            A.CallTo(() => configuration.MaintenanceMode).Returns(true);
            
            var lconfig = new FilterConfig(configuration);

            lconfig.Collection.Count(x => x.GetType() == (typeof(MaintenanceModeFilter))).Should().Be(1);
        }

        [Fact]
        public void FilterConfig_GivenNotMaintenanceMode_ShouldNotContainMaintenanceModeFilter()
        {
            A.CallTo(() => configuration.MaintenanceMode).Returns(false);

            config.Collection.Count(x => x.GetType() == (typeof(MaintenanceModeFilter))).Should().Be(0);
        }
    }
}

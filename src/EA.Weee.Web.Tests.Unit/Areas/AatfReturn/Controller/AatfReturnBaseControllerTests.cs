namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Controllers.Base;
    using Xunit;

    public class AatfReturnBaseControllerTests
    {
        [Fact]
        public void CheckAatfTaskListControllerInheritsExternalSiteController()
        {
            typeof(AatfReturnBaseController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void AatfReturnBaseController_ShouldHaveNoCache()
        {
            var attribute = typeof(AatfReturnBaseController).GetCustomAttribute<OutputCacheAttribute>();

            attribute.NoStore.Should().BeTrue();
            attribute.Duration.Should().Be(0);
            attribute.VaryByParam.Should().Be("None");
        }
    }
}

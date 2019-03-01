namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
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
            var attributes = typeof(AatfReturnBaseController).GetCustomAttributes(typeof(OutputCacheAttribute), false);

            attributes
        }
    }
}

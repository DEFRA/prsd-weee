namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using FluentAssertions;
    using Web.Areas.Test.Controllers;
    using Xunit;

    public class ManageCacheControllerTests
    {
        [Fact]
        public void ManageCacheController_ShouldHaveTestControllerBase()
        {
            typeof(ManageCacheController).BaseType.Name.Should().Be(typeof(TestControllerBase).Name);
        }
    }
}

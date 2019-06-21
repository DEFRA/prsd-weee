namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using FluentAssertions;
    using Web.Areas.Test.Controllers;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void HomeController_ShouldHaveTestControllerBase()
        {
            typeof(HomeController).BaseType.Name.Should().Be(typeof(TestControllerBase).Name);
        }
    }
}

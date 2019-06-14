namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using FluentAssertions;
    using Web.Areas.Test.Controllers;
    using Xunit;

    public class ManagePcsReturnsSubmissionWindowControllerTests
    {
        [Fact]
        public void ManagePcsReturnsSubmissionWindowController_ShouldHaveTestControllerBase()
        {
            typeof(ManagePcsReturnsSubmissionWindowController).BaseType.Name.Should().Be(typeof(TestControllerBase).Name);
        }
    }
}

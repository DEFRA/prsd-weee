namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using FluentAssertions;
    using Web.Areas.Test.Controllers;
    using Xunit;

    public class CreatePcsMemberXmlFileControllerTests
    {
        [Fact]
        public void CreatePcsMemberXmlFileController_ShouldHaveTestControllerBase()
        {
            typeof(CreatePcsMemberXmlFileController).BaseType.Name.Should().Be(typeof(TestControllerBase).Name);
        }
    }
}

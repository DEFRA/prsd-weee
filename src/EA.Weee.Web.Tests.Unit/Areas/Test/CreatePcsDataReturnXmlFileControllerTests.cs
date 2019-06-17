namespace EA.Weee.Web.Tests.Unit.Areas.Test
{
    using FluentAssertions;
    using Web.Areas.Test.Controllers;
    using Xunit;

    public class CreatePcsDataReturnXmlFileControllerTests
    {
        [Fact]
        public void CreatePcsDataReturnXmlFileController_ShouldHaveTestControllerBase()
        {
            typeof(CreatePcsDataReturnXmlFileController).BaseType.Name.Should().Be(typeof(TestControllerBase).Name);
        }
    }
}

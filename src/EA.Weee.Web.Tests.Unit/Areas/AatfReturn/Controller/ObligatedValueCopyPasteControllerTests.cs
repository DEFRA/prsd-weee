namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using FluentAssertions;
    using Xunit;

    public class ObligatedValueCopyPasteControllerTests
    {
        [Fact]
        public void CheckObligatedValueCopyPasteControllerInheritsExternalSiteController()
        {
            typeof(ObligatedValueCopyPasteController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }
    }
}

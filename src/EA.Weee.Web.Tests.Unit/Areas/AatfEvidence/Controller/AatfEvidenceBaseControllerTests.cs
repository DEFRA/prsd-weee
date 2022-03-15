namespace EA.Weee.Web.Tests.Unit.Areas.AatfEvidence.Controller
{
    using FluentAssertions;
    using Web.Areas.AatfEvidence.Controllers;
    using Web.Controllers.Base;
    using Xunit;

    public class AatfReturnEvidenceBaseControllerTests
    {
        [Fact]
        public void CheckAatfTaskListControllerInheritsExternalSiteController()
        {
            typeof(AatfEvidenceBaseController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System.Web.Mvc;
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

        [Fact]
        public void AatfEvidenceBaseController_ShouldHaveNoCache()
        {
            typeof(AatfEvidenceBaseController).Should().BeDecoratedWith<OutputCacheAttribute>(a =>
                a.NoStore == true &&
                a.Duration == 0 &&
                a.VaryByParam.Equals("None"));
        }
    }
}

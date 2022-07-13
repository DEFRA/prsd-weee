namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Controllers.Base;
    using FluentAssertions;
    using Xunit;

    public class BalancingSchemeEvidenceBaseControllerTests
    {
        [Fact]
        public void CheckSchemeEvidenceBaseControllerInheritsExternalSiteController()
        {
            typeof(BalancingSchemeEvidenceBaseController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }

        [Fact]
        public void Controller_IsDecoratedWith_ValidatePBSEvidenceNotesEnabled()
        {
            typeof(BalancingSchemeEvidenceBaseController).Should()
                .BeDecoratedWith<ValidatePBSEvidenceNotesEnabledAttribute>(a => a.Match(new ValidatePBSEvidenceNotesEnabledAttribute()));
        }
    }
}

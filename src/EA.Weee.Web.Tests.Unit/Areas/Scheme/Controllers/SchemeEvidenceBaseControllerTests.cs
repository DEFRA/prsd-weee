namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Controllers.Base;
    using FluentAssertions;
    using Xunit;

    public class SchemeEvidenceBaseControllerTests
    {
        [Fact]
        public void CheckSchemeEvidenceBaseControllerInheritsExternalSiteController()
        {
            typeof(SchemeEvidenceBaseController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }

        [Fact]
        public void CheckSchemeEvidenceBaseControllerHasAttributeValidatePcsEvidenceEnabled()
        {
            typeof(SchemeEvidenceBaseController).Should().BeDecoratedWith<ValidateSchemeEvidenceEnabledAttribute>();
        }

        [Fact]
        public void SchemeEvidenceBaseController_ShouldHaveNoCache()
        {
            typeof(SchemeEvidenceBaseController).Should().BeDecoratedWith<OutputCacheAttribute>(a =>
                a.NoStore == true &&
                a.Duration == 0 && 
                a.VaryByParam.Equals("None"));
        }
    }
}

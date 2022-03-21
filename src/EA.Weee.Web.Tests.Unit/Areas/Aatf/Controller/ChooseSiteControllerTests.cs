namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using Api.Client;
    using AutoFixture;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.AatfEvidence.Controllers;
    using Xunit;

    public class ChooseSiteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ChooseSiteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Fixture fixture;

        public ChooseSiteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            fixture = new Fixture();

//            controller = new SelectYourAatfController();
        }

        [Fact]
        public void SelectYourPcsControllerInheritsExternalSiteController()
        {
            typeof(ChooseSiteController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }
    }
}
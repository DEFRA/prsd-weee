namespace EA.Weee.Web.Tests.Unit.Areas.AatfEvidence.Controller
{
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using Web.Areas.AatfEvidence.Controllers;
    using Xunit;

    public class NoteControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly NoteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Fixture fixture;

        public NoteControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            fixture = new Fixture();

            controller = new NoteController();
        }

        [Fact]
        public void NoteControllerInheritsExternalSiteController()
        {
            typeof(NoteController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }

        [Fact]
        public void CreateGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = controller.Create() as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }
    }
}
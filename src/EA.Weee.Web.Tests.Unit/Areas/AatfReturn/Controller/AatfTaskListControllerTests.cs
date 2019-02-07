namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfTaskListControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly AatfTaskListController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public AatfTaskListControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            controller = new AatfTaskListController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void CheckAatfTaskListControllerInheritsExternalSiteController()
        {
            typeof(NonObligatedController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

    }
}

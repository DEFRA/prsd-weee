namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.Controllers;
    using Xunit;

    public class AeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly AeController controller;
        private readonly UrlHelper urlHelper;

        public AeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            urlHelper = A.Fake<UrlHelper>();

            controller = new AeController(() => weeeClient, breadcrumbService);
        }

        [Fact]
        public void ManageAesControllerInheritsAdminController()
        {
            typeof(AeController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }

        [Fact]
        public async Task ManageAes_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var result = await controller.ManageAes();

            breadcrumbService.InternalActivity.Should().Be("Manage AEs");
        }

        [Fact]
        public async void ManageAesGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.ManageAes() as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void ManageAesGet_AesShouldBeRetrieved()
        {
            var result = await controller.ManageAes();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfs>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

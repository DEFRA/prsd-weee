namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Aatf;
    using Xunit;

    public class AatfControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IMapper mapper;
        public AatfControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            weeeCache = A.Fake<IWeeeCache>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            AatfController controller = CreateController();

            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.ManageAatfs(new ManageAatfsViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        private AatfController CreateController()
        {
            return new AatfController(() => weeeClient, weeeCache, breadcrumbService, mapper);
        }

        [Fact]
        public async Task ManageAatfsPost_ReturnsSelectedGuid()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = CreateController();

            var result = await controller.ManageAatfs(new ManageAatfsViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("Details", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["Id"]);
        }

        [Fact]
        public async Task ManageAatfPost_ModelError_GetAatfsMustBeRun()
        {
            AatfController controller = CreateController();

            controller.ModelState.AddModelError(string.Empty, "Validation message");

            await controller.ManageAatfs(new ManageAatfsViewModel());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfs>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAatfsList_Always_SetsInternalBreadcrumbToManageAATFs()
        {
            AatfController controller = CreateController();

            ActionResult result = await controller.ManageAatfs();

            Assert.Equal("Manage AATFs", breadcrumbService.InternalActivity);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_BreadcrumbShouldBeSet()
        {
            AatfController controller = CreateController();

            var aatfData = A.Fake<AatfData>();
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            await controller.Details(A.Dummy<Guid>());

            Assert.Equal(breadcrumbService.InternalActivity, InternalUserActivity.ManageAatfs);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_ViewModelShouldBeCreatedWithApprovalDate()
        {
            AatfController controller = CreateController();
            AatfDetailsViewModel viewModel = A.Fake<AatfDetailsViewModel>();

            var aatfData = A.Fake<AatfData>();
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            var result = await controller.Details(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfIdButNoApprovalDate_ViewModelShouldBeCreatedWithNullApprovalDate()
        {
            AatfController controller = CreateController();
            AatfDetailsViewModel viewModel = A.Fake<AatfDetailsViewModel>();
            viewModel.ApprovalDate = null;

            var aatfData = A.Fake<AatfData>();
            aatfData.ApprovalDate = default(DateTime);
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            var result = await controller.Details(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }
    }
}

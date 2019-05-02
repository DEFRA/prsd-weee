namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Aatf;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
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
            Assert.Equal("AATFdetails", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["Id"]);
        }
    }
}

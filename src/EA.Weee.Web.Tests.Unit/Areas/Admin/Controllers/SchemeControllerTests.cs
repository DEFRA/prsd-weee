namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Xunit;

    public class SchemeControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;

        public SchemeControllerTests()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            apiClient = () => weeeClient;
        }

        [Fact]
        public async Task ManageSchemesPost_AllGood_ReturnsManageSchemeRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = new SchemeController(apiClient);

            var result = await controller.ManageSchemes(new ManageSchemesViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("ManageScheme", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["schemeId"]);
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            var controller = new SchemeController(apiClient);
            controller.ModelState.AddModelError(string.Empty, "Some error");

            var result = await controller.ManageSchemes(new ManageSchemesViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}

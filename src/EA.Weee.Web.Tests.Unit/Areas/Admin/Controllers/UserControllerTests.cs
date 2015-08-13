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

    public class UserControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;

        public UserControllerTests()
        {
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            apiClient = () => weeeClient;
        }

        [Fact]
        public async Task ManageUsersPost_ReturnsEditUserRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = new UserController(apiClient);

            var result = await controller.ManageUsers(new ManageUsersViewModel { SelectedUserId = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("EditUser", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["userId"]);
        }
    }
}

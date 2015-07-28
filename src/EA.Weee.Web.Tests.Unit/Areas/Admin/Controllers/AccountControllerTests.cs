namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using FakeItEasy;
    using Prsd.Core.Web.ApiClient;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async void HttpPost_Create_ModelIsInvalid_ShouldReturnViewWithModel()
        {
            var controller = AccountController();
            controller.ModelState.AddModelError("Key", "Something went wrong :(");

            var model = ValidModel();
            var result = await controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ShouldSubmitUserDetailsOnce_AndOnlyRoleShouldBeInternalUser_AndShouldRedirectToActivationRequiredPage()
        {
            var model = ValidModel();
            var newUser = A.Fake<INewUser>();

            var userCreationData = new UserCreationData();
            A.CallTo(() => newUser.CreateUserAsync(A<UserCreationData>._))
                .Invokes((UserCreationData u) => userCreationData = u)
                .Returns(Task.FromResult(A<string>._));

            A.CallTo(() => apiClient.NewUser).Returns(newUser);

            var result = await AccountController().Create(model);

            A.CallTo(() => apiClient.NewUser.CreateUserAsync(A<UserCreationData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(model.Name, userCreationData.FirstName);
            Assert.Equal(model.Surname, userCreationData.Surname);
            Assert.Equal(model.Email, userCreationData.Email);
            Assert.Equal(model.Password, userCreationData.Password);

            Assert.Single(userCreationData.Roles);
            Assert.Equal(UserRole.InternalUser, userCreationData.Roles.Single());

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("UserAccountActivationRequired", redirectValues["action"]);
            Assert.Equal("Account", redirectValues["controller"]);
            Assert.Equal(string.Empty, redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ApiThrowsException_ShouldNotCatch()
        {
            var model = ValidModel();

            var newUser = A.Fake<INewUser>();
            A.CallTo(() => newUser.CreateUserAsync(A<UserCreationData>._))
                .Throws(new ApiException(HttpStatusCode.BadRequest, new ApiError()));
            A.CallTo(() => apiClient.NewUser).Returns(newUser);

            await Assert.ThrowsAnyAsync<ApiException>(() => AccountController().Create(model));
        }

        private InternalUserCreationViewModel ValidModel()
        {
            return new InternalUserCreationViewModel
            {
                ConfirmPassword = "Password*99",
                Password = "Password*99",
                Email = "test@environment-agency.gov.uk",
                Name = "test",
                Surname = "name"
            };
        }

        private AccountController AccountController()
        {
            return new AccountController(() => apiClient);
        }
    }
}

namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Core;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.OAuth;
    using Thinktecture.IdentityModel.Client;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClient oauthClient;
        private readonly IAuthenticationManager authenticationManager;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClient = A.Fake<IOAuthClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
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

        [Fact] public async void HttpPost_Create_ModelIsValid_ShouldIncludeUserDetails_AndOnlyClaimShouldBeInternalAccess()
        {
            var model = ValidModel();
            var newUser = A.Fake<INewUser>();

            var userCreationData = new UserCreationData();
            A.CallTo(() => newUser.CreateUserAsync(A<UserCreationData>._))
                .Invokes((UserCreationData u) => userCreationData = u)
                .Returns(Task.FromResult(A<string>._));

            A.CallTo(() => apiClient.NewUser).Returns(newUser);
            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            await AccountController().Create(model);

            Assert.Equal(model.Name, userCreationData.FirstName);
            Assert.Equal(model.Surname, userCreationData.Surname);
            Assert.Equal(model.Email, userCreationData.Email);
            Assert.Equal(model.Password, userCreationData.Password);

            Assert.Single(userCreationData.Claims);
            Assert.Equal(Claims.CanAccessInternalArea, userCreationData.Claims.Single());
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ShouldSubmitUserDetailsOnce_AndShouldRequestTokenOnce_AndShouldSignInOnce_AndShouldRedirectToAccountActivationPage()
        {
            var model = ValidModel();
            var newUser = A.Fake<INewUser>();

            A.CallTo(() => apiClient.NewUser).Returns(newUser);
            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            var result = await AccountController().Create(model);

            A.CallTo(() => apiClient.NewUser.CreateUserAsync(A<UserCreationData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => authenticationManager.SignIn(A<ClaimsIdentity>._))
                .MustHaveHappened(Repeated.Exactly.Once);

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
            var context = A.Fake<HttpContextBase>();
            var controller = new AccountController(() => apiClient, () => oauthClient, authenticationManager);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            return controller;
        }
    }
}

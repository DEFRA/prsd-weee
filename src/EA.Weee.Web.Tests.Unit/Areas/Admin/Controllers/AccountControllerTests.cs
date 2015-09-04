namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Claims;
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
    using Prsd.Core.Web.OpenId;
    using Services;
    using Thinktecture.IdentityModel.Client;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClient oauthClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IUserInfoClient userInfoClient;
        private readonly IExternalRouteService externalRouteService;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClient = A.Fake<IOAuthClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            userInfoClient = A.Fake<IUserInfoClient>();
            externalRouteService = A.Fake<IExternalRouteService>();
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
        public async void HttpPost_Create_ModelIsValid_ShouldIncludeUserDetails()
        {
            var model = ValidModel();
            var newUser = A.Fake<IUnauthenticatedUser>();

            var userCreationData = new InternalUserCreationData();
            A.CallTo(() => newUser.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .Invokes((InternalUserCreationData u) => userCreationData = u)
                .Returns(Task.FromResult(A<string>._));

            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            await AccountController().Create(model);

            Assert.Equal(model.Name, userCreationData.FirstName);
            Assert.Equal(model.Surname, userCreationData.Surname);
            Assert.Equal(model.Email, userCreationData.Email);
            Assert.Equal(model.Password, userCreationData.Password);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ShouldSubmitUserDetailsOnce_AndShouldRequestTokenOnce_AndShouldSignInOnce_AndShouldRedirectToAccountActivationPage()
        {
            var model = ValidModel();
            var newUser = A.Fake<IUnauthenticatedUser>();

            A.CallTo(() => apiClient.User).Returns(newUser);
            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            var result = await AccountController().Create(model);

            A.CallTo(() => apiClient.User.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => authenticationManager.SignIn(A<ClaimsIdentity>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AdminAccountActivationRequired", redirectValues["action"]);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ApiThrowsException_ShouldNotCatch()
        {
            var model = ValidModel();

            var newUser = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => newUser.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .Throws(new ApiException(HttpStatusCode.BadRequest, new ApiError()));
            A.CallTo(() => apiClient.User).Returns(newUser);

            await Assert.ThrowsAnyAsync<ApiException>(() => AccountController().Create(model));
        }

        [Fact]
        public void HttpGet_SignIn_ShouldReturnsLoginView()
        {
            var controller = AccountController();
            var result = controller.SignIn("AnyUrl");
            var viewResult = ((ViewResult)result);
            Assert.Equal("SignIn", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = AccountController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };
            var result = await controller.SignIn(model, "AnyUrl");

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void HttpPost_AdminAccountActivationRequired_IfUserResendsActivationEmail_ShouldSendActivationEmail()
        {
            // Arrange
            IUnauthenticatedUser user = A.Fake<IUnauthenticatedUser>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.User).Returns(user);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(OAuth2Constants.AccessToken, "accessToken"));
            
            IAuthenticationManager authenticationManager = A.Fake<IAuthenticationManager>();
            A.CallTo(() => authenticationManager.User).Returns(new ClaimsPrincipal(identity));

            IExternalRouteService externalRouteService = A.Fake<IExternalRouteService>();
            A.CallTo(() => externalRouteService.ActivateInternalUserAccountUrl).Returns("activationBaseUrl");

            AccountController controller = new AccountController(
                () => weeeClient,
                authenticationManager,
                () => A.Fake<IOAuthClient>(),
                () => A.Fake<IUserInfoClient>(),
                externalRouteService);

            FormCollection formCollection = new FormCollection();

            // Act
            await controller.AdminAccountActivationRequired(formCollection);

            // Assert
            A.CallTo(() => user.ResendActivationEmail("accessToken", "activationBaseUrl"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void AdminAccount_IfNotActivated_ShouldRedirectToAdminAccountActivationRequired()
        {
            Guid id = Guid.NewGuid();
            string code =
                "LZHQ5TGVPA6FtUb6AmSssW6o8GpGtkMzRJTP4%2bhK9CGitEafOHBRGriU%2b7ruHbAq85Btymlnu1ewPxkIZGE17v98a21EPTaCNE1N2QlD%2b5FDgwULWlC28SS%2fKpFRIEXD9RaaYjSS6%2bfyvyexihUGKskaqaTB4%2f%2b4bRcZ%2fniu%2bqCNT%2fSY6ziGbvkNRX9oM%2fXW";

            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code }))
               .Returns(false);

            var result = await AccountController().ActivateUserAccount(id, code);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("AdminAccountActivationRequired", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void AdminAccount_ActiveUserAccount_ActivatesTheAccount()
        {
            await AccountController().ActivateUserAccount(A<Guid>._, A<string>._);

            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValidAndUserInfoResponseClaimToCanAccessInternalArea_ShouldRequestTokenOnce_AndShouldGetUserInfoOnce_AndShouldRedirectToHomePage()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            var controller = AccountController();

            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            var userInfoResponse = new UserInfoResponse(HttpStatusCode.Accepted, string.Empty)
            {
                Claims = new[] { new Tuple<string, string>(string.Empty, Claims.CanAccessInternalArea) }
            };

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .Returns(userInfoResponse);

            var result = await controller.SignIn(model, "AnyUrl");

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", redirectValues["action"]);
            Assert.Equal("Home", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValidAndUserInfoResponseClaimToCanAccessExternalArea_ShouldRequestTokenOnce_AndShouldGetUserInfoOnce_AndShouldReturnLoginView()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            var controller = AccountController();

            A.CallTo(() => oauthClient.GetAccessTokenAsync(A<string>._, A<string>._))
                .Returns(A.Fake<TokenResponse>());

            var userInfoResponse = new UserInfoResponse(HttpStatusCode.Accepted, string.Empty)
            {
                Claims = new[] { new Tuple<string, string>(string.Empty, Claims.CanAccessExternalArea) }
            };

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .Returns(userInfoResponse);

            var result = await controller.SignIn(model, "AnyUrl");

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var viewResult = ((ViewResult)result);

            Assert.Equal("SignIn", viewResult.ViewName);
            Assert.False(controller.ModelState.IsValid);
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
            var request = GetFakeRequest();

            var context = A.Fake<HttpContextBase>();
            A.CallTo(() => context.Request).Returns(request);

            var controller = new AccountController(
                () => apiClient,
                authenticationManager,
                () => oauthClient,
                () => userInfoClient,
                externalRouteService);

            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            controller.Url = new UrlHelper(new RequestContext(controller.HttpContext, new RouteData()));

            return controller;
        }

        private HttpRequestBase GetFakeRequest()
        {
            var request = A.Fake<HttpRequestBase>();
            var url = new Uri("https://fakeurl.com");

            A.CallTo(() => request.Url).Returns(url);

            return request;
        }
    }
}

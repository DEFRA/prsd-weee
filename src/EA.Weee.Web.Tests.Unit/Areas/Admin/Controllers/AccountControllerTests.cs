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
        private readonly IEmailService emailService;
        private readonly IUserInfoClient userInfoClient;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClient = A.Fake<IOAuthClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            emailService = A.Fake<IEmailService>();
            userInfoClient = A.Fake<IUserInfoClient>();
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
        public async void HttpPost_Create_ModelIsValid_ShouldIncludeUserDetails_AndOnlyClaimShouldBeInternalAccess()
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

            Assert.Equal("AdminAccountActivationRequired", redirectValues["action"]);
            Assert.Equal("Account", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ShouldSendEmail()
        {
            var model = ValidModel();

            await AccountController().Create(model);

            A.CallTo(() => emailService.GenerateUserAccountActivationMessage(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsInvalid_ShouldNotSendEmail()
        {
            var model = ValidModel();
            var controller = AccountController();
            controller.ModelState.AddModelError("Key", "Something went wrong :(");

            await controller.Create(model);

            A.CallTo(() => emailService.GenerateUserAccountActivationMessage(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
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

        [Fact]
        public void HttpGet_Login_ShouldReturnsLoginView()
        {
            var controller = AccountController();
            var result = controller.Login("AnyUrl");
            var viewResult = ((ViewResult)result);
            Assert.Equal("Login", viewResult.ViewName);
        }

        [Fact]
        public async void HttpPost_Login_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = AccountController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };
            var result = await controller.Login(model, "AnyUrl");

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void HttpPost_AdminAccountActivationRequired_IfUserResendsActivationEmail_ShouldSendActivationEmail()
        {
            var newUser = A.Fake<INewUser>();
            var controller = AccountController();

            var fakeUrlHelper = A.Fake<UrlHelper>();
            controller.Url = fakeUrlHelper;
            string route = "/Admin/Account/ActivateUserAccount";
            A.CallTo(() => fakeUrlHelper.Action(A<string>.Ignored, A<string>.Ignored)).Returns(route);

            A.CallTo(() => apiClient.NewUser).Returns(newUser);

            A.CallTo(
                () =>
                    emailService.GenerateUserAccountActivationMessage(A<string>._, A<string>._, A<string>._, A<string>._))
                .Returns(new MailMessage());

            await controller.AdminAccountActivationRequired(A<FormCollection>._);

            A.CallTo(() => emailService.SendAsync(A<MailMessage>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void AdminAccount_IfNotActivated_ShouldRedirectToAdminAccountActivationRequired()
        {
            Guid id = Guid.NewGuid();
            string code =
                "LZHQ5TGVPA6FtUb6AmSssW6o8GpGtkMzRJTP4%2bhK9CGitEafOHBRGriU%2b7ruHbAq85Btymlnu1ewPxkIZGE17v98a21EPTaCNE1N2QlD%2b5FDgwULWlC28SS%2fKpFRIEXD9RaaYjSS6%2bfyvyexihUGKskaqaTB4%2f%2b4bRcZ%2fniu%2bqCNT%2fSY6ziGbvkNRX9oM%2fXW";

            A.CallTo(() => apiClient.NewUser.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code }))
               .Returns(false);

            var result = await AccountController().ActivateUserAccount(id, code);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("AdminAccountActivationRequired", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void AdminAccount_ActiveUserAccount_ActivatesTheAccount()
        {
            await AccountController().ActivateUserAccount(A<Guid>._, A<string>._);

            A.CallTo(() => apiClient.NewUser.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_Login_ModelIsValidAndUserInfoResponseClaimToCanAccessInternalArea_ShouldRequestTokenOnce_AndShouldGetUserInfoOnce_AndShouldRedirectToHomePage()
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

            var result = await controller.Login(model, "AnyUrl");

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", redirectValues["action"]);
            Assert.Equal("Home", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_Login_ModelIsValidAndUserInfoResponseClaimToCanAccessExternalArea_ShouldRequestTokenOnce_AndShouldGetUserInfoOnce_AndShouldReturnLoginView()
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

            var result = await controller.Login(model, "AnyUrl");

            A.CallTo(() => oauthClient.GetAccessTokenAsync(model.Email, model.Password))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => userInfoClient.GetUserInfoAsync(A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var viewResult = ((ViewResult)result);

            Assert.Equal("Login", viewResult.ViewName);
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

            var controller = new AccountController(() => apiClient, authenticationManager, emailService, () => oauthClient, () => userInfoClient);
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

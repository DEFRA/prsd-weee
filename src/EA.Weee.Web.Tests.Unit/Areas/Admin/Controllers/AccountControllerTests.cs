namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Authorization;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Mediator;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.Shared;
    using Thinktecture.IdentityModel.Client;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Account;
    using Weee.Requests.Admin;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClient oauthClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IUserInfoClient userInfoClient;
        private readonly IExternalRouteService externalRouteService;
        private readonly IWeeeAuthorization weeeAuthorization;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClient = A.Fake<IOAuthClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            userInfoClient = A.Fake<IUserInfoClient>();
            externalRouteService = A.Fake<IExternalRouteService>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
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

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshadjk"));

            await AccountController().Create(model);

            Assert.Equal(model.Name, userCreationData.FirstName);
            Assert.Equal(model.Surname, userCreationData.Surname);
            Assert.Equal(model.Email, userCreationData.Email);
            Assert.Equal(model.Password, userCreationData.Password);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ShouldSubmitUserDetailsOnce_AndShouldSignInOnce()
        {
            var model = ValidModel();
            var newUser = A.Fake<IUnauthenticatedUser>();

            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshadjk"));

            await AccountController().Create(model);

            A.CallTo(() => apiClient.User.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_Create_SignInIsUnsuccessful_ShouldAddErrorToModel_AndReturnViewWithModel()
        {
            var model = ValidModel();
            const string loginError = "Oops";
            var newUser = A.Fake<IUnauthenticatedUser>();

            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Fail(loginError));

            var controller = AccountController();
            var result = await controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)result).Model);

            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Equal(loginError, controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_Create_SignInSuccessful_ShouldAddUserToCompetentAuthority_AndRedirectToAccountActivation()
        {
            var newUser = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjk"));

            var result = await AccountController().Create(ValidModel());

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<RedirectToRouteResult>(result);

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

            A.CallTo(() => apiClient.User).Returns(user);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(OAuth2Constants.AccessToken, "accessToken"));
            
            A.CallTo(() => authenticationManager.User).Returns(new ClaimsPrincipal(identity));
            A.CallTo(() => externalRouteService.ActivateInternalUserAccountUrl).Returns("activationBaseUrl");

            FormCollection formCollection = new FormCollection();

            // Act
            await AccountController().AdminAccountActivationRequired(formCollection);

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
        public async void HttpPost_SignIn_ModelIsValid_AndSignInSucceeds_ShouldRedirectToRedirectMechanism()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };
          
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
              .Returns(LoginResult.Success("dsadsada"));

            var result = await AccountController().SignIn(model, "AnyUrl");

            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", redirectValues["action"]);
            Assert.Equal("Account", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValid_AndSignInSucceeds_UserActive_ShouldRedirectToRedirectAuthorisationRequired()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            var result = await AccountController().InternalUserAuthorisationRequired();

            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", redirectValues["action"]);
            Assert.Equal("Home", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValid_AndSignInSucceeds_UserPending_ShouldRedirectToRedirectAuthorisationRequired()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dsadsada"));

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Pending);

            var result = await AccountController().SignIn(model, "AnyUrl");
            
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", redirectValues["action"]);
            Assert.Equal("Account", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValid_AndSignInSucceeds_UserRejected_ShouldRedirectToRedirectAuthorisationRequired()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dsadsada"));

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Rejected);

            var result = await AccountController().SignIn(model, "AnyUrl");

            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("InternalUserAuthorisationRequired", redirectValues["action"]);
            Assert.Equal("Account", redirectValues["controller"]);
            Assert.Equal("Admin", redirectValues["area"]);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValid_ButSignInFails_ShouldAddModelError_AndReturnViewWithModel()
        {
            var loginError = ":(";
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Fail(loginError));

            var controller = AccountController();
            var result = await controller.SignIn(model, "AnyUrl");

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)result).Model);

            Assert.False(controller.ModelState.IsValid);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Equal(loginError, controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsInvalid_ReturnsViewWithModel()
        {
            // Arrange
            var controller = AccountController();
            controller.ModelState.AddModelError("Some model property", "Some error occurred");

            var passwordResetModel = new ResetPasswordModel();

            // Act
            ActionResult result = await controller.ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_CallsApiToResetPassword()
        {
            // Arrange
            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult(A<string>._));

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjkal"));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            var passwordResetModel = new ResetPasswordModel();

            // Act
            ActionResult result = await AccountController().ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            // Assert
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_PasswordResetThrowsApiBadRequestExceptionWithModelErrors_ReturnsViewWithModel_AndErrorAddedToModelState()
        {
            // Arrange
            Dictionary<string, ICollection<string>> modelState = new Dictionary<string, ICollection<string>>
            {
                {
                    "A Key", new List<string>
                    {
                        "Something wen't wrong"
                    }
                }
            };

            ApiBadRequestException badRequestException = new ApiBadRequestException(HttpStatusCode.BadRequest, new ApiBadRequest
            {
                ModelState = modelState
            });

            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Throws(badRequestException);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            AccountController controller = AccountController();

            ResetPasswordModel passwordResetModel = new ResetPasswordModel();

            // Act
            ActionResult result = await controller.ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Contains("Something wen't wrong", controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_AndAuthorizationSuccessful_ShouldRedirectToSignIn()
        {
            // Arrange
            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult("an@email.address"));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            // Act
            ActionResult result = await AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel());

            // Assert
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("SignIn", routeValues["action"]);
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
                externalRouteService,
                weeeAuthorization);

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

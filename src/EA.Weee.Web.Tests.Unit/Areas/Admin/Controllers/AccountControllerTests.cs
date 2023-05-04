namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Authorization;
    using Core.Routing;
    using Core.Shared;
    using FakeItEasy;
    using IdentityModel;
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
    using FluentAssertions;
    using IdentityModel.Client;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Areas.Admin.ViewModels.Account;
    using Weee.Requests.Admin;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClientCredentialClient oauthClientCredential;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IExternalRouteService externalRouteService;
        private readonly IWeeeAuthorization weeeAuthorization;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClientCredential = A.Fake<IOAuthClientCredentialClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            externalRouteService = A.Fake<IExternalRouteService>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
        }

        [Fact]
        public void AccountController_ShouldInheritFromAdminBaseController()
        {
            typeof(AccountController).Should().BeDerivedFrom<AdminController>();
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
            var token = A.Fake<TokenResponse>();
            var accessToken = "token";

            A.CallTo(() => oauthClientCredential.GetClientCredentialsAsync()).Returns(token);

            var userCreationData = new InternalUserCreationData();
            A.CallTo(() => newUser.CreateInternalUserAsync(A<InternalUserCreationData>._, token.AccessToken))
                .Invokes((InternalUserCreationData u, string t) =>
                {
                    userCreationData = u;
                    accessToken = token.AccessToken;
                })
                .Returns(Task.FromResult(A.Dummy<string>()));

            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshadjk", A.Dummy<ActionResult>()));

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

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshadjk", A.Dummy<ActionResult>()));

            await AccountController().Create(model);

            A.CallTo(() => apiClient.User.CreateInternalUserAsync(A<InternalUserCreationData>._, A<string>._))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void HttpPost_Create_SignInIsUnsuccessful_ShouldAddErrorToModel_AndReturnViewWithModel()
        {
            var model = ValidModel();
            const string loginError = "Oops";
            var newUser = A.Fake<IUnauthenticatedUser>();

            A.CallTo(() => apiClient.User).Returns(newUser);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
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

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjk", A.Dummy<ActionResult>()));

            var result = await AccountController().Create(ValidModel());

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AdminAccountActivationRequired", redirectValues["action"]);
        }

        [Fact]
        public async void HttpPost_Create_ModelIsValid_ApiThrowsException_ShouldNotCatch()
        {
            var model = ValidModel();

            var newUser = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => newUser.CreateInternalUserAsync(A<InternalUserCreationData>._, A<string>._))
                .Throws(new ApiException(HttpStatusCode.BadRequest, new ApiError()));
            A.CallTo(() => apiClient.User).Returns(newUser);

            await Assert.ThrowsAnyAsync<ApiException>(() => AccountController().Create(model));
        }

        [Fact]
        public async void HttpGet_SignIn_IsNotSignedIn_ShouldReturnLoginView()
        {
            A.CallTo(() => weeeAuthorization.GetAuthorizationState())
                .Returns(AuthorizationState.NotLoggedIn());

            var controller = AccountController();
            var result = await controller.SignIn("AnyUrl");
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
            identity.AddClaim(new Claim(OidcConstants.TokenTypes.AccessToken, "accessToken"));

            A.CallTo(() => authenticationManager.User).Returns(new ClaimsPrincipal(identity));
            A.CallTo(() => externalRouteService.ActivateInternalUserAccountUrl).Returns("activationBaseUrl");

            FormCollection formCollection = new FormCollection();

            // Act
            await AccountController().AdminAccountActivationRequired(formCollection);

            // Assert
            A.CallTo(() => user.ResendActivationEmail("accessToken", "activationBaseUrl"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task ActivateUserAccount_WithInvalidToken_ReturnsAccountActivationFailedView()
        {
            // Arrange
            IWeeeClient apiClient = A.Fake<IWeeeClient>();
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, A<string>._))
                .Returns(false);

            IExternalRouteService externalRouteService = A.Dummy<IExternalRouteService>();
            IAuthenticationManager authenticationManager = A.Dummy<IAuthenticationManager>();
            IWeeeAuthorization weeeAuthorization = A.Dummy<IWeeeAuthorization>();

            var controller = new AccountController(() => apiClient, authenticationManager, externalRouteService, weeeAuthorization, () => oauthClientCredential);

            // Act
            var result = await controller.ActivateUserAccount(new Guid("EF565DF2-DC16-4589-9CE4-B29568B3E274"), "code");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("AccountActivationFailed", viewResult.ViewName);
        }

        [Fact]
        public async Task AdminAccount_ActiveUserAccount_ActivatesTheAccount()
        {
            // Arrange
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, A<string>._))
                .Returns(true);

            // Act
            var result = await AccountController().ActivateUserAccount(A.Dummy<Guid>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, A<string>._))
                .MustHaveHappened(1, Times.Exactly);

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("AccountActivated", viewResult.ViewName);
        }

        [Fact]
        public async Task ActivateUserAccount_InvokesApiWithCorrectParameters()
        {
            // Arrange
            var controller = AccountController();

            var userId = Guid.NewGuid();
            string code = "Code";
            var accessToken = "token";

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            A.CallTo(() => externalRouteService.ViewCompetentAuthorityUserRoute)
                 .Returns(viewUserRoute);

            ActivatedUserAccountData activatedUserAccountData = null;

            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, A<string>._))
                .Invokes((ActivatedUserAccountData a, string b) =>
                {
                    activatedUserAccountData = a;
                    accessToken = b;
                })
                .Returns(true);

            // Act
            await controller.ActivateUserAccount(userId, code);

            // Assert
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, accessToken))
                .MustHaveHappened(1, Times.Exactly);

            Assert.NotNull(activatedUserAccountData);
            Assert.Equal(userId, activatedUserAccountData.Id);
            Assert.Equal(code, activatedUserAccountData.Code);
            Assert.Same(viewUserRoute, activatedUserAccountData.ViewUserRoute);
        }

        [Fact]
        public async void HttpPost_SignIn_ModelIsValid_AndSignInSucceeds_ShouldRedirectToDefaultLoginAction()
        {
            var model = new InternalLoginViewModel
            {
                Email = "test@sepa.org.uk",
                Password = "Test123***",
                RememberMe = false
            };

            var defaultLoginAction = A.Dummy<ActionResult>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAdminUserStatus>._)).Returns(UserStatus.Active);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
              .Returns(LoginResult.Success("dsadsada", defaultLoginAction));

            var result = await AccountController().SignIn(model, string.Empty);

            Assert.Equal(defaultLoginAction, result);
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

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
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
            ActionResult result = await controller.ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_CallsApiToResetPassword()
        {
            // Arrange
            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Returns(true);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjkal", A.Dummy<ActionResult>()));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            var passwordResetModel = new ResetPasswordModel();

            // Act
            ActionResult result = await AccountController().ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            // Assert
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .MustHaveHappened(1, Times.Exactly);
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
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Throws(badRequestException);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            AccountController controller = AccountController();

            ResetPasswordModel passwordResetModel = new ResetPasswordModel();

            // Act
            ActionResult result = await controller.ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Contains("Something wen't wrong", controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_AndAuthorizationSuccessful_ReturnsPasswordResetCompleteView()
        {
            // Arrange
            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Returns(true);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            // Act
            ActionResult result = await AccountController().ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), new ResetPasswordModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("ResetPasswordComplete", viewResult.ViewName);
        }

        [Fact]
        public void HttpGet_InternalUserAuthorisationRequired_NoStatusIsSupplied_ThrowsInvalidException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => AccountController().InternalUserAuthorisationRequired(null));
        }

        [Fact]
        public void HttpGet_InternalUserAuthorisationRequired_ActiveStatusIsSupplied_RedirectsToHomeIndex()
        {
            var result = AccountController().InternalUserAuthorisationRequired(UserStatus.Active);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Theory]
        [InlineData(UserStatus.Inactive)]
        [InlineData(UserStatus.Rejected)]
        [InlineData(UserStatus.Pending)]
        public void HttpGet_InternalUserAuthorisationRequired_StatusOtherThanActiveIsSupplied_ReturnsViewWithStatus(UserStatus userStatus)
        {
            var result = AccountController().InternalUserAuthorisationRequired(userStatus);

            Assert.IsType<ViewResult>(result);

            var model = ((ViewResult)result).Model;

            Assert.IsType<InternalUserAuthorizationRequiredViewModel>(model);
            Assert.Equal(userStatus, ((InternalUserAuthorizationRequiredViewModel)model).Status);
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
                weeeAuthorization,
                () => oauthClientCredential);

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

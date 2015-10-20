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

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshadjk", A<ActionResult>._));

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
                .Returns(LoginResult.Success("dshadjk", A<ActionResult>._));

            await AccountController().Create(model);

            A.CallTo(() => apiClient.User.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
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
                .Returns(LoginResult.Success("dshjk", A<ActionResult>._));

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
        public async Task ActivateUserAccount_WithInvalidToken_ReturnsAccountActivationFailedView()
        {
            // Arrange
            IWeeeClient apiClient = A.Fake<IWeeeClient>();
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._))
                .Returns(false);

            IExternalRouteService externalRouteService = A.Dummy<IExternalRouteService>();
            IAuthenticationManager authenticationManager = A.Dummy<IAuthenticationManager>();
            IWeeeAuthorization weeeAuthorization = A.Dummy<IWeeeAuthorization>();

            var controller = new AccountController(() => apiClient, authenticationManager, externalRouteService, weeeAuthorization);

            // Act
            var result = await controller.ActivateUserAccount(new Guid("EF565DF2-DC16-4589-9CE4-B29568B3E274"), "code");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("AccountActivationFailed", viewResult.ViewName);
        }

        [Fact]
        public async void AdminAccount_ActiveUserAccount_ActivatesTheAccount()
        {
            await AccountController().ActivateUserAccount(A<Guid>._, A<string>._);

            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._))
                .MustHaveHappened(Repeated.Exactly.Once);
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

            var defaultLoginAction = A<ActionResult>._;

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
                .Returns(true);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjkal", A<ActionResult>._));

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
        public async void HttpPost_ResetPassword_ModelIsValid_AndAuthorizationSuccessful_ReturnsPasswordResetCompleteView()
        {
            // Arrange
            IUnauthenticatedUser unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(true);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            // Act
            ActionResult result = await AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("ResetPasswordComplete", viewResult.ViewName);
        }

        [HttpGet]
        public void HttpGet_InternalUserAuthorisationRequired_NoStatusIsSupplied_ThrowsInvalidException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => AccountController().InternalUserAuthorisationRequired(null));
        }

        [HttpGet]
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

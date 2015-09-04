namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using ViewModels.Account;
    using Web.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IOAuthClient oauthClient;
        private readonly IUserInfoClient userInfoClient;
        private readonly IUnauthenticatedUser unauthenticatedUserClient;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IExternalRouteService externalRouteService;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            oauthClient = A.Fake<IOAuthClient>();
            userInfoClient = A.Fake<IUserInfoClient>();
            unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            externalRouteService = A.Fake<IExternalRouteService>();
        }

        private AccountController AccountController()
        {
            return new AccountController(
                () => oauthClient,
                authenticationManager,
                () => apiClient,
                weeeAuthorization,
                () => userInfoClient,
                externalRouteService);
        }

        [Fact]
        public async void UserAccount_IfNotActivated_ShouldRedirectToUserAccountActivationRequired()
        {
            Guid id = Guid.NewGuid();
            string code =
                "LZHQ5TGVPA6FtUb6AmSssW6o8GpGtkMzRJTP4%2bhK9CGitEafOHBRGriU%2b7ruHbAq85Btymlnu1ewPxkIZGE17v98a21EPTaCNE1N2QlD%2b5FDgwULWlC28SS%2fKpFRIEXD9RaaYjSS6%2bfyvyexihUGKskaqaTB4%2f%2b4bRcZ%2fniu%2bqCNT%2fSY6ziGbvkNRX9oM%2fXW";

            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code }))
               .Returns(false);

            var result = await AccountController().ActivateUserAccount(id, code);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("UserAccountActivationRequired", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsInvalid_ReturnsViewWithModel()
        {
            var passwordResetModel = new ResetPasswordModel();

            var controller = AccountController();
            controller.ModelState.AddModelError("Some model property", "Some error occurred");

            var result = await controller.ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_CallsApiToResetPassword()
        {
            var passwordResetModel = new ResetPasswordModel();

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult(A<string>._));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            await AccountController().ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_PasswordResetThrowsApiBadRequestExceptionWithModelErrors_ReturnsViewWithModel_AndErrorAddedToModelState()
        {
            var passwordResetModel = new ResetPasswordModel();
            const string errorMessage = "Something wen't wrong";

            var modelState = new Dictionary<string, ICollection<string>>
            {
                {
                    "A Key", new List<string>
                    {
                        errorMessage
                    }
                }
            };

            var badRequestException = new ApiBadRequestException(HttpStatusCode.BadRequest, new ApiBadRequest
            {
                ModelState = modelState
            });

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Throws(badRequestException);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            var controller = AccountController();

            var result = await controller.ResetPassword(A<Guid>._, A<string>._, passwordResetModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Contains(errorMessage, controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_PasswordResetReturnsEmailAddress_ShouldSignUserIn_AndRedirectToRedirectReturnedByAuthorizationSignIn()
        {
            var redirect = new RedirectToRouteResult(new RouteValueDictionary(new { action = "AnAction", controller = "AController", area = "AnArea" }));

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult("an@email.address"));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._, A<string>._))
                .Returns(redirect);

            var result = await AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel());

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(redirect, result);
        }
    }
}

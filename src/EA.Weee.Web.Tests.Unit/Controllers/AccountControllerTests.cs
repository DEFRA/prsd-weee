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
    using Authorization;
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
        private readonly IUnauthenticatedUser unauthenticatedUserClient;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IExternalRouteService externalRouteService;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            externalRouteService = A.Fake<IExternalRouteService>();
        }

        private AccountController AccountController()
        {
            return new AccountController(
                () => apiClient,
                weeeAuthorization,
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

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjkal"));

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
        public async void HttpPost_ResetPassword_ModelIsValid_ButAuthorizationUnsuccessful_ShouldAddErrorToModelErrors_AndReturnView()
        {
            const string errorMessage = "Some error";
            var model = new ResetPasswordModel();

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult("an@email.address"));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Fail(errorMessage));

            var controller = AccountController();

            var result = await controller.ResetPassword(A<Guid>._, A<string>._, model);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)result).Model);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Contains(errorMessage, controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_AndAuthorizationSuccessful_ShouldRedirectToRedirectProcess()
        {
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(new PasswordResetResult("an@email.address"));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dsadsa"));

            var result = await AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel());

            A.CallTo(() => weeeAuthorization.SignIn(A<LoginType>._, A<string>._, A<string>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("RedirectProcess", routeValues["action"]);
            Assert.Equal("Account", routeValues["controller"]);
        }
    }
}

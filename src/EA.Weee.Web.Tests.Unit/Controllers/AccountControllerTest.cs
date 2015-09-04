namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Core.Organisations;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using ViewModels.Account;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class AccountControllerTest
    {
        private readonly IWeeeClient apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;
        private readonly IOAuthClient oauthClient;
        private readonly IUserInfoClient userInfoClient;
        private readonly IUnauthenticatedUser unauthenticatedUserClient;

        public AccountControllerTest()
        {
            apiClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            oauthClient = A.Fake<IOAuthClient>();
            emailService = A.Fake<EmailService>();
            userInfoClient = A.Fake<IUserInfoClient>();
            unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
        }

        private AccountController AccountController()
        {
            return new AccountController(() => oauthClient, authenticationManager, () => apiClient, emailService, () => userInfoClient);
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
                .Returns(true);

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

            var modelState = new Dictionary<string, ICollection<string>>();
            modelState.Add("A Key", new List<string>
            {
                errorMessage
            });

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
        public async void HttpPost_ResetPassword_ModelIsValid_PasswordResetReturnsFalse_ThrowsException()
        {
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(false);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            await Assert.ThrowsAnyAsync<Exception>(
                () => AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel()));
        }

        [Fact]
        public async void HttpPost_ResetPassword_ModelIsValid_PasswordResetReturnsTrue_RedirectsToRedirectProcess()
        {
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._))
                .Returns(true);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            var result = await AccountController().ResetPassword(A<Guid>._, A<string>._, new ResetPasswordModel());

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Account", routeValues["controller"]);
            Assert.Equal("RedirectProcess", routeValues["action"]);
        }
    }
}

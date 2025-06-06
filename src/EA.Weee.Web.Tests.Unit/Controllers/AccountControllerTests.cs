﻿namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Authorization;
    using EA.Weee.Web.ViewModels.Account;
    using FakeItEasy;
    using Prsd.Core.Web.ApiClient;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Prsd.Core.Web.OAuth;
    using Web.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IOAuthClientCredentialClient oauthClientCredentialClient;
        private readonly IUnauthenticatedUser unauthenticatedUserClient;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IExternalRouteService externalRouteService;

        public AccountControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            oauthClientCredentialClient = A.Fake<IOAuthClientCredentialClient>();
            unauthenticatedUserClient = A.Fake<IUnauthenticatedUser>();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            externalRouteService = A.Fake<IExternalRouteService>();
        }

        private AccountController AccountController()
        {
            return new AccountController(
                () => apiClient,
                weeeAuthorization,
                externalRouteService,
                () => oauthClientCredentialClient);
        }

        [Fact]
        public async Task ActivateUserAccount_WithInvalidToken_ReturnsAccountActivationFailedView()
        {
            // Arrange
            IWeeeClient apiClient = A.Fake<IWeeeClient>();
            A.CallTo(() => apiClient.User.ActivateUserAccountEmailAsync(A<ActivatedUserAccountData>._, A<string>._))
                .Returns(false);

            IWeeeAuthorization weeeAuthorization = A.Dummy<IWeeeAuthorization>();
            IExternalRouteService externalRouteService = A.Dummy<IExternalRouteService>();

            var controller = new AccountController(() => apiClient, weeeAuthorization, externalRouteService, () => oauthClientCredentialClient);

            // Act
            var result = await controller.ActivateUserAccount(new Guid("EF565DF2-DC16-4589-9CE4-B29568B3E274"), "code");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("AccountActivationFailed", viewResult.ViewName);
        }

        [Fact]
        public async Task HttpPost_ResetPassword_ModelIsInvalid_ReturnsViewWithModel()
        {
            var passwordResetModel = new ResetPasswordModel();

            var controller = AccountController();
            controller.ModelState.AddModelError("Some model property", "Some error occurred");

            var result = await controller.ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
        }

        [Fact]
        public async Task HttpPost_ResetPassword_ModelIsValid_CallsApiToResetPassword()
        {
            var passwordResetModel = new ResetPasswordModel();

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Returns(true);

            A.CallTo(() => weeeAuthorization.SignIn(A<string>._, A<string>._, A<bool>._))
                .Returns(LoginResult.Success("dshjkal", A.Dummy<ActionResult>()));

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            await AccountController().ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HttpPost_ResetPassword_ModelIsValid_PasswordResetThrowsApiBadRequestExceptionWithModelErrors_ReturnsViewWithModel_AndErrorAddedToModelState()
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

            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Throws(badRequestException);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            var controller = AccountController();

            var result = await controller.ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), passwordResetModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(passwordResetModel, ((ViewResult)result).Model);
            Assert.Single(controller.ModelState.Values);
            Assert.Single(controller.ModelState.Values.Single().Errors);
            Assert.Contains(errorMessage, controller.ModelState.Values.Single().Errors.Single().ErrorMessage);
        }

        [Fact]
        public async Task HttpPost_ResetPassword_ModelIsValid_AndAuthorizationSuccessful_ReturnsResetPasswordCompleteView()
        {
            // Arrange
            A.CallTo(() => unauthenticatedUserClient.ResetPasswordAsync(A<PasswordResetData>._, A<string>._))
                .Returns(true);

            A.CallTo(() => apiClient.User)
                .Returns(unauthenticatedUserClient);

            // Act
            var result = await AccountController().ResetPassword(A.Dummy<Guid>(), A.Dummy<string>(), new ResetPasswordModel());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("ResetPasswordComplete", viewResult.ViewName);
        }
    }
}

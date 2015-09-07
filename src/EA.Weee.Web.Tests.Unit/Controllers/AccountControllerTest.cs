namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Core.Organisations;
    using EA.Weee.Web.ViewModels.Account;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class AccountControllerTest
    {
        private readonly IWeeeClient apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IOAuthClient oauthClient;
        private readonly IUserInfoClient userInfoClient;
        private readonly IExternalRouteService externalRouteService;

        public AccountControllerTest()
        {
            apiClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            oauthClient = A.Fake<IOAuthClient>();
            userInfoClient = A.Fake<IUserInfoClient>();
            externalRouteService = A.Fake<IExternalRouteService>();
        }

        private AccountController AccountController()
        {
            return new AccountController(
                () => oauthClient,
                authenticationManager,
                () => apiClient,
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
        public async void HttpPost_ResetPasswordRequest_EmailNotInDatabase_ReturnsError()
        {
            var model = new ResetPasswordRequestViewModel
            {
                Email = "a@b.c"
            };

            A.CallTo(() => apiClient.User.ResetPasswordRequestAsync(A<string>._)).Returns(false);

            var controller = AccountController();
            var result = await controller.ResetPasswordRequest(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void HttpPost_ResetPasswordRequest_EmailNotInDatabase_ReturnsResetPasswordInstructionView()
        {
            var model = new ResetPasswordRequestViewModel
            {
                Email = "a@b.c"
            };

            A.CallTo(() => apiClient.User.ResetPasswordRequestAsync(A<string>._)).Returns(true);

            var controller = AccountController();
            var result = await controller.ResetPasswordRequest(model);
            var viewResult = (ViewResult)result;

            Assert.Equal("ResetPasswordInstruction", viewResult.ViewName);
            Assert.Equal(model.Email, viewResult.ViewBag.Email);
        }
    }
}

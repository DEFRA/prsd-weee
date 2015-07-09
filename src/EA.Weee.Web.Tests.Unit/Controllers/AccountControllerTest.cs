namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Core.Organisations;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Services;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class AccountControllerTest
    {
        private readonly IWeeeClient apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;
        private readonly IOAuthClient oauthClient;

        public AccountControllerTest()
        {
            apiClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            oauthClient = A.Fake<IOAuthClient>();
            emailService = A.Fake<EmailService>();
        }

        private AccountController AccountController()
        {
            return new AccountController(() => oauthClient, authenticationManager, () => apiClient, emailService);
        }

        [Fact]
        public async void GetRedirectProcess_NoOrganisationUsers_ShouldRedirectToType()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationsByUserId>._))
                .Returns(new List<OrganisationUserData>());

            var result = await AccountController().RedirectProcess();
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Type", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void GetRedirectProcess_ApprovedOrganisationUserOne_ShouldRedirectToChooseActivity()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetOrganisationsByUserId>._))
                .Returns(new List<OrganisationUserData> { new OrganisationUserData() });

            var result = await AccountController().RedirectProcess();
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ChooseActivity", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void UserAccount_IfNotActivated_ShouldRedirectToUserAccountActivationRequired()
        {
             Guid id = Guid.NewGuid();
            string code =
                "LZHQ5TGVPA6FtUb6AmSssW6o8GpGtkMzRJTP4%2bhK9CGitEafOHBRGriU%2b7ruHbAq85Btymlnu1ewPxkIZGE17v98a21EPTaCNE1N2QlD%2b5FDgwULWlC28SS%2fKpFRIEXD9RaaYjSS6%2bfyvyexihUGKskaqaTB4%2f%2b4bRcZ%2fniu%2bqCNT%2fSY6ziGbvkNRX9oM%2fXW";
            
            A.CallTo(() => apiClient.NewUser.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code }))
               .Returns(false);
           
            var result = await AccountController().ActivateUserAccount(id, code);
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("UserAccountActivationRequired", redirectToRouteResult.RouteValues["action"]);
        }
    }
}

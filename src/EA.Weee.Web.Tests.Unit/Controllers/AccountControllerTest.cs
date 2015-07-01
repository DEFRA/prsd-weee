namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Api.Client;
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
        public async void GetRedirectProcess_ApprovedOrganisationUserZero_ShouldRedirectToType()
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
                .Returns(new List<OrganisationUserData> { new OrganisationUserData{OrganisationUserStatus = OrganisationUserStatus.Approved} });

            var result = await AccountController().RedirectProcess();
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ChooseActivity", redirectToRouteResult.RouteValues["action"]);
        }
    }
}

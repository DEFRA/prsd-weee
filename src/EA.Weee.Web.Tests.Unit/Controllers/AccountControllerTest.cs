namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Web.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class AccountControllerTest
    {
        private readonly IWeeeClient apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IOAuthClient oauthClient;

        public AccountControllerTest()
        {
            apiClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            oauthClient = A.Fake<IOAuthClient>();
        }
        
        private AccountController AccountController()
        {
            return new AccountController(() => oauthClient, () => apiClient, authenticationManager);
        }

        [Fact]
        public async void GetRedirectProcess_ApprovedOrganisationUserZero_ShouldRedirectToType()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApprovedOrganisationsByUserId>._))
                .Returns(new List<OrganisationUserData>());

            var result = await AccountController().RedirectProcess();
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Type", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async void GetRedirectProcess_ApprovedOrganisationUserOne_ShouldRedirectToChooseActivity()
        {
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetApprovedOrganisationsByUserId>._))
                .Returns(new List<OrganisationUserData> { new OrganisationUserData() });

            var result = await AccountController().RedirectProcess();
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("ChooseActivity", redirectToRouteResult.RouteValues["action"]);
        }
    }
}

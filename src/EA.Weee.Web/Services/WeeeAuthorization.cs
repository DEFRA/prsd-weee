namespace EA.Weee.Web.Services
{
    using Prsd.Core.Web.OAuth;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OpenId;

    public class WeeeAuthorization : IWeeeAuthorization
    {
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IUserInfoClient> userInfoClient;

        public WeeeAuthorization(Func<IOAuthClient> oauthClient, IAuthenticationManager authenticationManager, Func<IUserInfoClient> userInfoClient)
        {
            this.oauthClient = oauthClient;
            this.authenticationManager = authenticationManager;
            this.userInfoClient = userInfoClient;
        }

        public async Task<ActionResult> SignIn(string emailAddress, string password, bool rememberMe, string returnUrl = "")
        {
            var response = await oauthClient().GetAccessTokenAsync(emailAddress, password);
            authenticationManager.SignIn(response.GenerateUserIdentity());

            var userInfo = await userInfoClient().GetUserInfoAsync(response.AccessToken);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return new RedirectResult(returnUrl);
            }

            if (userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessInternalArea))
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new { action = "ChooseActivity", controller = "Home", area = "Admin" }));
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new { area = string.Empty, controller = "Organisation", action = "Index" }));
        }

        public ActionResult SignOut(IPrincipal user)
        {
            authenticationManager.SignOut();

            if (((ClaimsIdentity)user.Identity).HasClaim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea))
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new { area = "Admin", controller = "Account", action = "SignIn" }));
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new { area = string.Empty, controller = "Account", action = "SignIn" }));
        }
    }
}
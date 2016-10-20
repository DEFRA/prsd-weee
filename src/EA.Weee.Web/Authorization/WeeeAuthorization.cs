namespace EA.Weee.Web.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using IdentityModel;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Security;
    
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

        public async Task<AuthorizationState> GetAuthorizationState()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return AuthorizationState.NotLoggedIn();
            }

            var accessToken = HttpContext.Current.User.GetAccessToken();

            return AuthorizationState.LoggedIn(accessToken, await GetLoginAction(accessToken));
        }

        public async Task<LoginResult> SignIn(string emailAddress, string password, bool rememberMe)
        {
            if (!(await GetAuthorizationState()).IsLoggedIn)
            {
                var response = await oauthClient().GetAccessTokenAsync(emailAddress, password);

                if (response.AccessToken == null)
                {
                    return LoginResult.Fail(ParseLoginError(response.Error));
                }

                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, response.GenerateUserIdentity());
                return LoginResult.Success(response.AccessToken, await GetLoginAction(response.AccessToken));
            }

            var accessToken = HttpContext.Current.User.GetAccessToken();
            return LoginResult.Success(accessToken, await GetLoginAction(accessToken));
        }

        public void SignOut()
        {
            authenticationManager.SignOut();
        }

        private async Task<ActionResult> GetLoginAction(string accessToken)
        {
            var userInfo = await userInfoClient().GetUserInfoAsync(accessToken);
            if (userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessInternalArea))
            {
                return new RedirectToRouteResult("InternalLogin", null);
            }

            return new RedirectToRouteResult("Login", null);
        }

        private string ParseLoginError(string error)
        {
            switch (error)
            {
                case OidcConstants.AuthorizeErrors.AccessDenied:
                    return "Access denied";
                case OidcConstants.TokenErrors.InvalidGrant:
                    return "Invalid credentials";
                case OidcConstants.AuthorizeResponse.Error:
                case OidcConstants.TokenErrors.UnsupportedGrantType:
                case OidcConstants.TokenErrors.InvalidClient:
                case OidcConstants.AuthorizeErrors.InvalidRequest:
                case OidcConstants.AuthorizeErrors.InvalidScope:
                case OidcConstants.AuthorizeErrors.UnauthorizedClient:
                case OidcConstants.AuthorizeErrors.UnsupportedResponseType:
                default:
                    return "Internal error";
            }
        }
    }
}
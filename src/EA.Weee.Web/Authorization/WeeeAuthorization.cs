namespace EA.Weee.Web.Authorization
{
    using EA.Prsd.Core;
    using EA.Weee.Web.App_Start;
    using IdentityModel;
    using Infrastructure;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Security;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using ClaimTypes = Prsd.Core.Web.ClaimTypes;

    public class WeeeAuthorization : IWeeeAuthorization
    {
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IUserInfoClient> userInfoClient;

        private HttpContext context = HttpContext.Current;

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

        public async Task<LoginResult> RefreshAuthentication()
        {
            if ((await GetAuthorizationState()).IsLoggedIn)
            {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;

                var expiresAt = claimsIdentity.FindFirst("sessionExpires");

                if (expiresAt != null)
                {
                    authenticationManager.SignOut();
                    return LoginResult.Fail("User has already extended");
                }

                var refreshToken = claimsIdentity.FindFirst(OidcConstants.AuthorizeResponse.RefreshToken);

                var response = await oauthClient().GetRefreshTokenAsync(refreshToken.Value);

                if (response.AccessToken == null)
                {
                    return LoginResult.Fail(ParseLoginError(response.Error));
                }

                // check existing identity if it has the session claim, should not be able to refresh again
                // Also store it so it can be restored after creating the new identity

                var newIdentity = response.GenerateUserIdentity();

                var expires = new DateTimeOffset(SystemTime.UtcNow).AddMinutes(10);

                newIdentity.AddClaim(new Claim("sessionExpires", expires.ToString("u")));
                authenticationManager.AuthenticationResponseGrant =
                    new AuthenticationResponseGrant(
                        new ClaimsPrincipal(newIdentity),
                        new AuthenticationProperties { IsPersistent = true, ExpiresUtc = expires });

                //authenticationManager.SignOut(Constants.WeeeAuthType);

                ReturnUrlMapping returnUrlMapping = new ReturnUrlMapping();
                returnUrlMapping.Add("/account/sign-out", null);
                returnUrlMapping.Add("/admin/account/sign-out", null);

                authenticationManager.SignIn(newIdentity);
            }

            return LoginResult.Fail("User not authentication");
        }

        public void SignOut()
        {
            context.Session["ShowFooterAndLinks"] = false;
            authenticationManager.SignOut();
        }

        private async Task<ActionResult> GetLoginAction(string accessToken)
        {
            context.Session["ShowFooterAndLinks"] = false;

            var userInfo = await userInfoClient().GetUserInfoAsync(accessToken);
            if (userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessInternalArea))
            {
                context.Session["ShowFooterAndLinks"] = true;
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
                    return "Incorrect email address or password";
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
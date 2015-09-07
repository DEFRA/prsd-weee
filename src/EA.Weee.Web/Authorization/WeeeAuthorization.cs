namespace EA.Weee.Web.Authorization
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Thinktecture.IdentityModel.Client;

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

        public async Task<LoginResult> SignIn(LoginType loginType, string emailAddress, string password, bool rememberMe)
        {
            var alreadySignedIn = HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated;

            if (!alreadySignedIn)
            {
                var response = await oauthClient().GetAccessTokenAsync(emailAddress, password);

                if (response.AccessToken == null)
                {
                    return LoginResult.Fail(this.ParseLoginError(response.Error));
                }

                if (loginType != await GetLoginType(response.AccessToken))
                {
                    return LoginResult.Fail("Invalid login details");
                }

                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, response.GenerateUserIdentity());
                return LoginResult.Success(response.AccessToken);
            }

            return LoginResult.Success(HttpContext.Current.User.GetAccessToken());
        }

        public void SignOut()
        {
            authenticationManager.SignOut();
        }

        private async Task<LoginType> GetLoginType(string accessToken)
        {
            var userInfo = await userInfoClient().GetUserInfoAsync(accessToken);
            if (userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessInternalArea))
            {
                return LoginType.Internal;
            }

            return LoginType.External;
        }

        private string ParseLoginError(string error)
        {
            switch (error)
            {
                case OAuth2Constants.Errors.AccessDenied:
                    return "Access denied";
                case OAuth2Constants.Errors.InvalidGrant:
                    return "Invalid credentials";
                case OAuth2Constants.Errors.Error:
                case OAuth2Constants.Errors.InvalidClient:
                case OAuth2Constants.Errors.InvalidRequest:
                case OAuth2Constants.Errors.InvalidScope:
                case OAuth2Constants.Errors.UnauthorizedClient:
                case OAuth2Constants.Errors.UnsupportedGrantType:
                case OAuth2Constants.Errors.UnsupportedResponseType:
                default:
                    return "Internal error";
            }
        }
    }
}
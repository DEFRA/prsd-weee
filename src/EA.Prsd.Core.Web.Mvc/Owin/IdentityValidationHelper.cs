namespace EA.Prsd.Core.Web.Mvc.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using IdentityModel;
    using Microsoft.Owin.Security.Cookies;
    using OAuth;
    using OpenId;
    using Web.Extensions;
    using ClaimTypes = Web.ClaimTypes;

    public static class IdentityValidationHelper
    {
        public static async Task UpdateAccessToken(CookieValidateIdentityContext context)
        {
            if (context.Identity == null || !context.Identity.IsAuthenticated)
            {
                return;
            }

            var expiresAt = context.Identity.FindFirst(ClaimTypes.ExpiresAt);
            if (expiresAt != null)
            {
                DateTime expiryDate;

                if (!DateTime.TryParseExact(expiresAt.Value, "u", CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal, out expiryDate))
                {
                    // If the expiry date can't be parsed then sign the user out.
                    RejectIdentity(context);
                    return;
                }

                if (expiryDate < SystemTime.UtcNow)
                {
                    // If the access token has expired, try and get a new one.
                    var refreshTokenClaim = context.Identity.FindFirst(OidcConstants.AuthorizeResponse.RefreshToken);
                    if (refreshTokenClaim != null)
                    {
                        var oauthClient = DependencyResolver.Current.GetService<IOAuthClient>();
                        var response = await oauthClient.GetRefreshTokenAsync(refreshTokenClaim.Value);

                        if (response.IsError)
                        {
                            // If the refresh token doesn't work (e.g. it is expired) then sign the user out.
                            RejectIdentity(context);
                            return;
                        }
                        
                        // Create a new cookie from the token response by signing out and in.
                        var identity = response.GenerateUserIdentity(context.Options.AuthenticationType);
                        var auth = context.OwinContext.Authentication;
                        auth.SignOut(context.Options.AuthenticationType);
                        auth.SignIn(context.Properties, identity);
                        context.ReplaceIdentity(identity);
                    }
                }
            }
        }

        public static async Task TransformClaims(CookieValidateIdentityContext context)
        {
            if (context.Identity == null || !context.Identity.IsAuthenticated)
            {
                return;
            }

            var accessTokenClaim = context.Identity.FindFirst(OidcConstants.AuthorizeResponse.AccessToken);

            if (accessTokenClaim == null)
            {
                RejectIdentity(context);
                return;
            }

            var userInfoClient = DependencyResolver.Current.GetService<IUserInfoClient>();

            var userInfo = await userInfoClient.GetUserInfoAsync(accessTokenClaim.Value);

            if (userInfo.IsError)
            {
                RejectIdentity(context);
                return;
            }

            var claims = new List<Claim>();
            userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

            claims.Add(accessTokenClaim);
            claims.Add(context.Identity.FindFirst(OidcConstants.AuthorizeResponse.RefreshToken));
            claims.Add(context.Identity.FindFirst(ClaimTypes.ExpiresAt));

            var nameId = context.Identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (nameId != null)
            {
                claims.Add(nameId);
            }

            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaims(claims);

            context.ReplaceIdentity(id);
        }

        private static void RejectIdentity(CookieValidateIdentityContext context)
        {
            context.RejectIdentity();
            context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
        }
    }
}
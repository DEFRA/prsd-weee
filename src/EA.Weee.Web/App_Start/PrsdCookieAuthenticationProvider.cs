namespace EA.Weee.Web.App_Start
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;    
    using System.Web.Mvc;
    using EA.Prsd.Core;   
    using EA.Prsd.Core.Web.OAuth;
    using EA.Prsd.Core.Web.OpenId;
    using Microsoft.Owin.Security.Cookies;
    using Prsd.Core.Web.Extensions;
    using Thinktecture.IdentityModel.Client;
    using ClaimTypes = Prsd.Core.Web.ClaimTypes;

    /// <summary>
    /// This class is taken from the PRSD project's class "PrsdCookieAuthenticationOptions".
    /// The code contained therein defines the behaviour of the provider rather than
    /// specifying any options.
    
    /// As WEEE needs to extend the common PRSD behaviours, I have duplicated these into
    /// a class which from which the WEEE provider can be derived.
    /// 
    /// Perhaps at some point this refactoring will make it back into the PRSD project.
    /// </summary>
    public class PrsdCookieAuthenticationProvider : CookieAuthenticationProvider
    {
        public PrsdCookieAuthenticationProvider()
        {
            OnValidateIdentity = (context) => { return this.OnValidateIdentityImpl(context); };
        }

        private async Task OnValidateIdentityImpl(CookieValidateIdentityContext context)
        {
            await UpdateAccessToken(context);

            await TransformClaims(context);
        }

        private async Task UpdateAccessToken(CookieValidateIdentityContext context)
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
                        DateTimeStyles.AssumeUniversal, out expiryDate))
                {
                    // If the expiry date can't be parsed then sign the user out.
                    RejectIdentity(context);
                    return;
                }

                if (expiryDate < SystemTime.UtcNow)
                {
                    // If the access token has expired, try and get a new one.
                    var refreshTokenClaim = context.Identity.FindFirst(OAuth2Constants.RefreshToken);
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

        private async Task TransformClaims(CookieValidateIdentityContext context)
        {
            if (context.Identity == null || !context.Identity.IsAuthenticated)
            {
                return;
            }

            var accessTokenClaim = context.Identity.FindFirst(OAuth2Constants.AccessToken);

            if (accessTokenClaim == null)
            {
                RejectIdentity(context);
                return;
            }

            var userInfoClient = DependencyResolver.Current.GetService<IUserInfoClient>();

            var userInfo = await userInfoClient.GetUserInfoAsync(accessTokenClaim.Value);

            if (userInfo.IsError || userInfo.IsHttpError)
            {
                RejectIdentity(context);
                return;
            }

            var claims = new List<Claim>();
            userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

            claims.Add(accessTokenClaim);
            claims.Add(context.Identity.FindFirst(OAuth2Constants.RefreshToken));
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

        private void RejectIdentity(CookieValidateIdentityContext context)
        {
            context.RejectIdentity();
            context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
        }
    }
}
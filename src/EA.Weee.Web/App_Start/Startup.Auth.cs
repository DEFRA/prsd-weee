namespace EA.Weee.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using Prsd.Core.Web.OAuth;
    using Services;
    using Thinktecture.IdentityModel.Client;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, AppConfiguration config)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Constants.WeeeAuthType,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = context => OnValidateIdentity(context, config)
                }
            });

            app.UseClaimsTransformation(incoming => TransformClaims(incoming, config));
        }

        private static async Task OnValidateIdentity(CookieValidateIdentityContext context, AppConfiguration config)
        {
            if (context.Identity == null || !context.Identity.IsAuthenticated)
            {
                return;
            }

            var expiresAt = context.Identity.FindFirst(Prsd.Core.Web.ClaimTypes.ExpiresAt);
            if (expiresAt != null && DateTime.Parse(expiresAt.Value) < DateTime.UtcNow)
            {
                var refreshTokenClaim = context.Identity.FindFirst(OAuth2Constants.RefreshToken);
                if (refreshTokenClaim != null)
                {
                    var oauthClient = new OAuthClient(config.ApiUrl, config.ApiSecret);
                    var response = await oauthClient.GetRefreshTokenAsync(refreshTokenClaim.Value);
                    var auth = context.OwinContext.Authentication;

                    if (response.IsError)
                    {
                        // If the refresh token doesn't work (e.g. it is expired) then sign the user out.
                        context.RejectIdentity();
                        auth.SignOut(context.Options.AuthenticationType);
                    }
                    else
                    {
                        // Create a new cookie from the token response by signing out and in.
                        auth.SignOut(context.Options.AuthenticationType);
                        auth.SignIn(context.Properties, response.GenerateUserIdentity());
                    }
                }
            }
        }

        private static async Task<ClaimsPrincipal> TransformClaims(ClaimsPrincipal incoming, AppConfiguration config)
        {
            if (!incoming.Identity.IsAuthenticated)
            {
                return incoming;
            }

            var claims = new List<Claim>();

            var userInfoClient = new UserInfoClient(
                new Uri(config.ApiUrl + "/connect/userinfo"),
                incoming.FindFirst(OAuth2Constants.AccessToken).Value);

            var userInfo = await userInfoClient.GetAsync();
            userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

            claims.Add(incoming.FindFirst(OAuth2Constants.AccessToken));
            claims.Add(incoming.FindFirst(OAuth2Constants.RefreshToken));
            claims.Add(incoming.FindFirst(Prsd.Core.Web.ClaimTypes.ExpiresAt));

            var nameId = incoming.FindFirst(ClaimTypes.NameIdentifier);
            if (nameId != null)
            {
                claims.Add(nameId);
            }

            var id = new ClaimsIdentity(Constants.WeeeAuthType);
            id.AddClaims(claims);

            return new ClaimsPrincipal(id);
        }
    }
}
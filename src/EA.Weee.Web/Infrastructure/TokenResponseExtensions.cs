namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Security.Claims;
    using Thinktecture.IdentityModel.Client;

    public static class TokenResponseExtensions
    {
        public static ClaimsIdentity GenerateUserIdentity(this TokenResponse response)
        {
            var identity = new ClaimsIdentity(Constants.WeeeAuthType);
            identity.AddClaim(new Claim(OAuth2Constants.AccessToken, response.AccessToken));

            if (response.RefreshToken != null)
            {
                identity.AddClaim(new Claim(OAuth2Constants.RefreshToken, response.RefreshToken));
            }

            identity.AddClaim(new Claim(Prsd.Core.Web.ClaimTypes.ExpiresAt,
                DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn).ToString()));

            return identity;
        }
    }
}
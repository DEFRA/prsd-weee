namespace EA.Prsd.Core.Web.Extensions
{
    using System;
    using System.Security.Claims;
    using IdentityModel;
    using IdentityModel.Client;
    using ClaimTypes = Web.ClaimTypes;

    public static class TokenResponseExtensions
    {
        public static ClaimsIdentity GenerateUserIdentity(this TokenResponse response, string authenticationType)
        {
            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaim(new Claim(OidcConstants.AuthorizeResponse.AccessToken, response.AccessToken));

            if (response.RefreshToken != null)
            {
                identity.AddClaim(new Claim(OidcConstants.AuthorizeResponse.RefreshToken, response.RefreshToken));
            }

            var offset = new DateTimeOffset(SystemTime.UtcNow);

            identity.AddClaim(new Claim(ClaimTypes.ExpiresAt,
                offset.AddSeconds(response.ExpiresIn).ToString("u")));

            return identity;
        }
    }
}
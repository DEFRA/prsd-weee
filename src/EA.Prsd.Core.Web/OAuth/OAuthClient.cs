namespace EA.Prsd.Core.Web.OAuth
{
    using System;
    using System.Threading.Tasks;
    using Thinktecture.IdentityModel.Client;

    public class OAuthClient : IOAuthClient
    {
        private readonly OAuth2Client oauth2Client;

        public OAuthClient(string baseUrl, string clientSecret)
        {
            var baseUri = new Uri(baseUrl);

            oauth2Client = new OAuth2Client(new Uri(baseUri, "/connect/token/"), "iws", clientSecret);
        }

        public async Task<TokenResponse> GetAccessTokenAsync(string username, string password)
        {
            return await oauth2Client.RequestResourceOwnerPasswordAsync(username, password,
                "openid api1 all_claims profile offline_access");
        }

        public async Task<TokenResponse> GetRefreshTokenAsync(string refreshToken)
        {
            return await oauth2Client.RequestRefreshTokenAsync(refreshToken);
        }
    }
}
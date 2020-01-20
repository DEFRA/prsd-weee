namespace EA.Prsd.Core.Web.OAuth
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class OAuthClientCredentialClient : OAuthClient, IOAuthClientCredentialClient
    {
        public OAuthClientCredentialClient(string baseUrl, string clientId, string clientSecret) : base(baseUrl, clientId, clientSecret)
        {
        }

        public new async Task<TokenResponse> GetClientCredentialsAsync()
        {
            return await Oauth2Client.RequestClientCredentialsAsync("api2");
        }
    }
}
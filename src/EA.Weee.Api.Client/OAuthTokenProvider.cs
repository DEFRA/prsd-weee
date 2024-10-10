namespace EA.Weee.Api.Client
{
    using Newtonsoft.Json;
    using Serilog;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class OAuthTokenProvider
    {
        private readonly IHttpClientWrapper httpClientWrapper;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string scope;
        private readonly string tenantId;

        public OAuthTokenProvider(
            IHttpClientWrapperFactory httpClientFactory,
            HttpClientHandlerConfig config,
            ILogger logger,
            string clientId,
            string clientSecret,
            string scope,
            string tenantId)
        {
            var tokenEndpoint = $"https://login.microsoftonline.com/{this.tenantId}/oauth2/v2.0/";
            httpClientWrapper = httpClientFactory.CreateHttpClient(tokenEndpoint, config, logger);
            //this.clientId = clientId;
            //this.clientSecret = clientSecret;
            //this.scope = scope;
            //this.tenantId = tenantId;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", scope)
            });

            var response = await httpClientWrapper.PostAsync("token", content);
            var responseContent2 = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            return tokenResponse.AccessToken;
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")] public string AccessToken { get; set; }
        }
    }
}
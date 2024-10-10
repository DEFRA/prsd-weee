namespace EA.Weee.Api.Client
{
    using System;
    using System.IdentityModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Api.Client.Serlializer;
    using Serilog;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;
        private readonly OAuthTokenProvider tokenProvider;
        private bool disposed;

        public CompaniesHouseClient(
            string baseUrl,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            ILogger logger,
            OAuthTokenProvider tokenProvider)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClientFactory).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(logger).IsNotNull();
            Condition.Requires(tokenProvider).IsNotNull();

            this.httpClient = httpClientFactory.CreateHttpClient(baseUrl, config, logger);
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
            this.tokenProvider = tokenProvider;
        }

        public async Task<DefraCompaniesHouseApiModel> GetCompanyDetailsAsync(string endpoint, string companyReference)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");
            if (!IsValidCompanyReference(companyReference))
            {
                return null;
            }

            try
            {
                var token = await tokenProvider.GetAccessTokenAsync();
                var requestUri = $"{endpoint}/{companyReference}";

                var response = await retryPolicy.ExecuteAsync(() =>
                    httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)
                    {
                        Headers = { { "Authorization", $"Bearer {token}" } }
                    })).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new BadRequestException("Companies house bad request");
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return jsonSerializer.Deserialize<DefraCompaniesHouseApiModel>(content);
            }
            catch (BadRequestException)
            {
                return null;
            }
        }

        private bool IsValidCompanyReference(string companyReference)
        {
            return !string.IsNullOrWhiteSpace(companyReference) && companyReference.All(char.IsDigit);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                (httpClient as IDisposable)?.Dispose();
            }
            disposed = true;
        }
    }
}
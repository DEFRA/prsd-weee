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
    using Serilog.Core;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;
        private readonly IOAuthTokenProvider tokenProvider;
        private readonly ILogger logger;

        private bool disposed;

        public CompaniesHouseClient(
            string baseUrl,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            ILogger logger,
            IOAuthTokenProvider tokenProvider)
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
            this.logger = logger;
        }

        public async Task<DefraCompaniesHouseApiModel> GetCompanyDetailsAsync(string endpoint, string companyReference)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");

            if (!IsValidCompanyReference(companyReference))
            {
                logger.Warning("Not calling companies house API invalid reference");

                return new DefraCompaniesHouseApiModel()
                {
                    InvalidReference = true
                };
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

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return jsonSerializer.Deserialize<DefraCompaniesHouseApiModel>(content);
            }
            catch (Exception ex)
            {
                logger.Error($"Error attempting to access companies house API for {companyReference}", ex);

                return new DefraCompaniesHouseApiModel()
                {
                    Error = true
                };
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
namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Api.Client.Serlializer;
    using Serilog;
    using System;
    using System.IdentityModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;

        private bool disposed;

        public CompaniesHouseClient(
            string baseUrl,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            X509Certificate2 certificate,
            ILogger logger)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClientFactory).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(certificate).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            this.httpClient = httpClientFactory.CreateHttpClientWithCertificate(baseUrl, config, logger, certificate);
            
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
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
                var response = await retryPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync($"{endpoint}/{companyReference}")).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new BadRequestException($"Companies house bad request");
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
                // Dispose of any disposable fields if necessary
            }
            disposed = true;
        }
    }
}
namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Serlializer;
    using System;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly HttpClient httpClient;
        private bool disposed;

        public CompaniesHouseClient(
            string baseUrl,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            X509Certificate2 certificate)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(certificate).IsNotNull();

            var handler = HttpClientHandlerFactory.Create(config);

            handler.ClientCertificates.Add(certificate);
            var baseUri = new Uri(baseUrl);

            httpClient = new HttpClient(handler)
            {
                BaseAddress = baseUri
            };

            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<T> GetCompanyDetailsAsync<T>(string endpoint, string companyReference)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");
            Condition.Requires(companyReference).IsNotNullOrWhiteSpace("Company reference cannot be null or whitespace.");

            var response = await retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"{endpoint}/{companyReference}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return jsonSerializer.Deserialize<T>(content);
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
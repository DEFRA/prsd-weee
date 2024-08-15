namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Serlializer;
    using System;
    using System.Threading.Tasks;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly string baseUrl;
        private bool disposed;

        public CompaniesHouseClient(
            string baseUrl,
            IHttpClientWrapper httpClient,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClient).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();

            this.baseUrl = baseUrl;
            this.httpClient = httpClient;
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<T> GetCompanyDetailsAsync<T>(string endpoint, string companyReference)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");
            Condition.Requires(companyReference).IsNotNullOrWhiteSpace("Company reference cannot be null or whitespace.");

            var response = await retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"{baseUrl}{endpoint}{companyReference}")).ConfigureAwait(false);

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
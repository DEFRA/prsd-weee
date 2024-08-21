namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Entities.AddressLookup;
    using EA.Weee.Api.Client.Serlializer;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AddressLookupClient : IAddressLookupClient
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;
        private readonly Dictionary<string, string> customHeaders;

        private bool disposed;

        public AddressLookupClient(
            string baseUrl,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            ILogger logger)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClientFactory).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            var handler = HttpClientHandlerFactory.Create(config);

            this.httpClient = httpClientFactory.CreateHttpClient(baseUrl, config, logger);
            
            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
            this.customHeaders = new Dictionary<string, string>();
            this.customHeaders.Add("Ocp-Apim-Subscription-Key", "e1cf75efc665494a8d1d792fe30f2319");
        }

        public async Task<AddressLookupResponse> GetAddressLookup(string endpoint, string postCode)
        {
            Condition.Requires(endpoint).IsNotNullOrWhiteSpace("Endpoint cannot be null or whitespace.");
            Condition.Requires(postCode).IsNotNullOrWhiteSpace("Postcode cannot be null or whitespace.");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}?postcode={postCode}");

            // Add custom headers to the request
            foreach (var header in customHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await retryPolicy.ExecuteAsync(() =>
                httpClient.SendAsync(request)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return jsonSerializer.Deserialize<AddressLookupResponse>(content);
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
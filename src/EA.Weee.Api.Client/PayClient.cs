namespace EA.Weee.Api.Client
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Api.Client.Serlializer;
    using Serilog;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class PayClient : IPayClient, IDisposable
    {
        private readonly IRetryPolicyWrapper retryPolicy;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClientWrapper httpClient;
        private readonly ILogger logger;

        private bool disposed;

        public PayClient(string baseUrl,
            string apiKey,
            IHttpClientWrapperFactory httpClientFactory,
            IRetryPolicyWrapper retryPolicy,
            IJsonSerializer jsonSerializer,
            HttpClientHandlerConfig config,
            ILogger logger)
        {
            Condition.Requires(baseUrl).IsNotNullOrWhiteSpace();
            Condition.Requires(apiKey).IsNotNullOrWhiteSpace();
            Condition.Requires(httpClientFactory).IsNotNull();
            Condition.Requires(config).IsNotNull();
            Condition.Requires(retryPolicy).IsNotNull();
            Condition.Requires(jsonSerializer).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            this.retryPolicy = retryPolicy;
            this.jsonSerializer = jsonSerializer;
            this.logger = logger;

            this.httpClient = httpClientFactory.CreateHttpClientWithAuthorization(baseUrl, config, logger, "Bearer", apiKey);
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(string idempotencyKey, CreateCardPaymentRequest body)
        {
            Condition.Requires(body).IsNotNull();

            try
            {
                var content = new StringContent(jsonSerializer.Serialize(body));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(idempotencyKey))
                {
                    content.Headers.Add("Idempotency-Key", idempotencyKey);
                }

                var response = await retryPolicy.ExecuteAsync(() =>
                    httpClient.PostAsync("v1/payments", content)).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return jsonSerializer.Deserialize<CreatePaymentResult>(responseContent);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while creating payment");
                throw;
            }
        }

        public async Task<PaymentWithAllLinks> GetPaymentAsync(string paymentId)
        {
            Condition.Requires(paymentId).IsNotNullOrWhiteSpace("Payment ID cannot be null or whitespace.");

            try
            {
                var response = await retryPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync($"v1/payments/{paymentId}")).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return jsonSerializer.Deserialize<PaymentWithAllLinks>(content);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while retrieving payment: {PaymentId}", paymentId);
                throw;
            }
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
                httpClient?.Dispose();
            }
            disposed = true;
        }
    }
}
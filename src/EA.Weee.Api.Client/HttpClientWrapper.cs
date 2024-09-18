namespace EA.Weee.Api.Client
{
    using Serilog;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public HttpClientWrapper(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            logger.Debug("Test debug log");
            return await SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var fullUrl = GetFullUrl(request.RequestUri);
                logger.Information("Starting {Method} request to {FullUrl}", request.Method, fullUrl);
                var response = await httpClient.SendAsync(request, cancellationToken);
                stopwatch.Stop();
                logger.Information(
                    "Completed {Method} request to {FullUrl} with status code {StatusCode} in {ElapsedMilliseconds} ms",
                    request.Method,
                    fullUrl,
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
                if (!response.IsSuccessStatusCode)
                {
                    logger.Warning(
                        "{Method} request to {FullUrl} returned non-success status code {StatusCode}",
                        request.Method,
                        fullUrl,
                        (int)response.StatusCode);
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                LogError(ex, "HTTP request exception", request, stopwatch);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                LogError(ex, "Request timed out", request, stopwatch);
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unexpected error", request, stopwatch);
                throw;
            }
        }

        private string GetFullUrl(Uri requestUri)
        {
            if (requestUri.IsAbsoluteUri)
            {
                return requestUri.ToString();
            }
            if (httpClient.BaseAddress == null)
            {
                return requestUri.ToString();
            }
            return new Uri(httpClient.BaseAddress, requestUri).ToString();
        }

        private void LogError(Exception ex, string errorType, HttpRequestMessage request, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var fullUrl = GetFullUrl(request.RequestUri);
            logger.Error(
                ex,
                "{ErrorType} occurred while making {Method} request to {FullUrl} after {ElapsedMilliseconds} ms: {ErrorMessage}",
                errorType,
                request.Method,
                fullUrl,
                stopwatch.ElapsedMilliseconds,
                ex.Message);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
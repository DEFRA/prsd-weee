namespace EA.Weee.Api.Client
{
    using Serilog;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public HttpClientWrapper(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                logger.Information("Starting GET request to {RequestUri}", requestUri);

                var response = await httpClient.GetAsync(requestUri);

                stopwatch.Stop();

                logger.Information(
                    "Completed GET request to {RequestUri} with status code {StatusCode} in {ElapsedMilliseconds} ms",
                    requestUri,
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    logger.Warning(
                        "GET request to {RequestUri} returned non-success status code {StatusCode}",
                        requestUri,
                        (int)response.StatusCode);
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();

                logger.Error(
                    ex,
                    "HTTP request exception occurred while making GET request to {RequestUri} after {ElapsedMilliseconds} ms: {ErrorMessage}",
                    requestUri,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();

                logger.Error(
                    ex,
                    "Request timed out while making GET request to {RequestUri} after {ElapsedMilliseconds} ms",
                    requestUri,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.Error(
                    ex,
                    "Unexpected error occurred while making GET request to {RequestUri} after {ElapsedMilliseconds} ms: {ErrorMessage}",
                    requestUri,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
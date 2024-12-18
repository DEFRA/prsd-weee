namespace EA.Weee.Api.Client.Polly
{
    using System;
    using System.Net.Http;
    using global::Polly;
    using global::Polly.Retry;
    using Serilog;

    public static class PollyPolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(IsTransientError)
                .Or<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timeSpan, retryCount, context) =>
                    {
                        logger.Information($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                    });
        }

        private static bool IsTransientError(HttpResponseMessage response)
        {
            int[] transientHttpStatusCodes = { 408, 429, 500, 502, 503, 504 };
            return Array.IndexOf(transientHttpStatusCodes, (int)response.StatusCode) != -1;
        }
    }
}

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
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
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
    }
}

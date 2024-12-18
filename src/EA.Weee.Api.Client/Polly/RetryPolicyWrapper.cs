namespace EA.Weee.Api.Client.Polly
{
    using global::Polly.Retry;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RetryPolicyWrapper : IRetryPolicyWrapper
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy;

        public RetryPolicyWrapper(AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            this.retryPolicy = retryPolicy;
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            if (typeof(TResult) == typeof(HttpResponseMessage))
            {
                var httpResponseMessageTask = action as Func<Task<HttpResponseMessage>>;
                if (httpResponseMessageTask != null)
                {
                    var result = await retryPolicy.ExecuteAsync(httpResponseMessageTask).ConfigureAwait(false);
                    return (TResult)(object)result;
                }
            }

            // For non-HttpResponseMessage types, execute without the policy
            return await action().ConfigureAwait(false);
        }
    }
}

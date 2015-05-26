namespace EA.Weee.Api.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Actions;
    using Newtonsoft.Json;
    using Prsd.Core.Mediator;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Extensions;

    public class WeeeClient : IWeeeClient
    {
        private readonly HttpClient httpClient;
        private INewUser newUser;

        public WeeeClient(string baseUrl)
        {
            var baseUri = new Uri(baseUrl);

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUri, "/api/")
            };
        }

        public INewUser NewUser
        {
            get { return newUser ?? (newUser = new NewUser(httpClient)); }
        }

        public async Task<TResult> SendAsync<TResult>(IRequest<TResult> request)
        {
            return await InternalSendAsync(request).ConfigureAwait(false);
        }

        public async Task<TResult> SendAsync<TResult>(string accessToken, IRequest<TResult> request)
        {
            httpClient.SetBearerToken(accessToken);
            return await InternalSendAsync(request).ConfigureAwait(false);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private async Task<TResult> InternalSendAsync<TResult>(IRequest<TResult> request)
        {
            var apiRequest = new ApiRequest
            {
                RequestJson = JsonConvert.SerializeObject(request),
                TypeName = request.GetType().AssemblyQualifiedName
            };

            var response = await httpClient.PostAsJsonAsync("Send", apiRequest).ConfigureAwait(false);

            var result = await response.CreateResponseAsync<TResult>().ConfigureAwait(false);

            return result;
        }
    }
}
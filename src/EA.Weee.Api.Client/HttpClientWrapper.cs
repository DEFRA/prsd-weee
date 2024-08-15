namespace EA.Weee.Api.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return httpClient.GetAsync(requestUri);
        }
    }
}

namespace EA.Prsd.Core.Web.Extensions
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string requestUri)
        {
            var result = await client.GetAsync(requestUri);
            return await result.Content.ReadAsAsync<T>();
        }

        public static async Task<T> GetAsync<T>(this HttpClient client, string accessToken, string requestUri)
        {
            client.SetBearerToken(accessToken);
            return await client.GetAsync<T>(requestUri);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string accessToken,
            string requestUri, T data)
        {
            client.SetBearerToken(accessToken);
            var result = await client.PostAsJsonAsync(requestUri, data);
            return result;
        }
    }
}
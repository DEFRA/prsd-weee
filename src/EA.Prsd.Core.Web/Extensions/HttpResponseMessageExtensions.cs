namespace EA.Prsd.Core.Web.Extensions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ApiClient;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> CreateResponseAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await httpResponseMessage.Content.ReadAsAsync<T>().ConfigureAwait(false);
            }

            var ex = await CreateApiException(httpResponseMessage);
            throw ex;
        }

        public static async Task<byte[]> CreateResponseByteArrayAsync(
            this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }

            var ex = await CreateApiException(httpResponseMessage);
            throw ex;
        }

        private static async Task<Exception> CreateApiException(HttpResponseMessage httpResponseMessage)
        {
            var response = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiBadRequestException(httpResponseMessage.StatusCode, JsonConvert.DeserializeObject<ApiBadRequest>(response));
            }

            return new ApiException(httpResponseMessage.StatusCode, JsonConvert.DeserializeObject<ApiError>(response));
        }
    }
}
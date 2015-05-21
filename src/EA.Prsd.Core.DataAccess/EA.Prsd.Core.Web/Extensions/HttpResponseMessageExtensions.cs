namespace EA.Prsd.Core.Web.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ApiClient;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        public static async Task<ApiResponse<T>> CreateResponseAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var result = new ApiResponse<T>(httpResponseMessage.StatusCode,
                    await httpResponseMessage.Content.ReadAsAsync<T>().ConfigureAwait(false));
                return result;
            }
            var httpErrorObject = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var anonymousErrorObject = new { Message = string.Empty, ModelState = new Dictionary<string, string[]>() };

            var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

            var listOfErrors = new List<string>();

            if (deserializedErrorObject.ModelState != null)
            {
                listOfErrors.AddRange(
                    deserializedErrorObject.ModelState
                        .Select(kvp => string.Join(". ", kvp.Value)));
            }
            else
            {
                var error =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);

                listOfErrors.AddRange(error.Select(kvp => kvp.Value));
            }

            return new ApiResponse<T>(httpResponseMessage.StatusCode, listOfErrors);
        }

        public static async Task<ApiResponse<byte[]>> CreateResponseByteArrayAsync(
            this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                return new ApiResponse<byte[]>(httpResponseMessage.StatusCode, bytes);
            }
            return new ApiResponse<byte[]>(httpResponseMessage.StatusCode,
                new[] { "Failed to generate the requested document." });
        }
    }
}
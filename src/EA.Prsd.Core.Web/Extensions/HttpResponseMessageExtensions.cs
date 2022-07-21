namespace EA.Prsd.Core.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using ApiClient;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> CreateResponseAsync<T>(this HttpResponseMessage httpResponseMessage,
            params JsonConverter[] converters)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<JsonConverter> localConverters = new List<JsonConverter>();
                if (converters != null)
                {
                    localConverters.AddRange(converters);
                }

                IEnumerable<MediaTypeFormatter> formatters = new[]
                {
                    new JsonMediaTypeFormatter
                    {
                        SerializerSettings = new JsonSerializerSettings
                        {
                            Converters = localConverters
                        }
                    }
                };

                return await httpResponseMessage.Content.ReadAsAsync<T>(formatters);
            }

            var ex = await CreateApiException(httpResponseMessage);
            throw ex;
        }

        private static async Task<Exception> CreateApiException(HttpResponseMessage httpResponseMessage)
        {
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiBadRequestException(httpResponseMessage.StatusCode, JsonConvert.DeserializeObject<ApiBadRequest>(response));
            }

            return new ApiException(httpResponseMessage.StatusCode, JsonConvert.DeserializeObject<ApiError>(response));
        }
    }
}
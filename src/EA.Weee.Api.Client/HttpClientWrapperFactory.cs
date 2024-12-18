namespace EA.Weee.Api.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using Serilog;

    public class HttpClientWrapperFactory : IHttpClientWrapperFactory
    {
        public IHttpClientWrapper CreateHttpClient(string baseUrl, HttpClientHandlerConfig config, ILogger logger)
        {
            return CreateHttpClientInternal(baseUrl, config, logger);
        }

        public IHttpClientWrapper CreateHttpClientWithCertificate(string baseUrl, HttpClientHandlerConfig config, ILogger logger, X509Certificate2 certificate)
        {
            return CreateHttpClientInternal(baseUrl, config, logger, certificate);
        }

        public IHttpClientWrapper CreateHttpClientWithAuthorization(string baseUrl, HttpClientHandlerConfig config, ILogger logger, string scheme, string parameter)
        {
            var handler = HttpClientHandlerFactory.Create(config);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(EnsureTrailingSlash(baseUrl))
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
            return new HttpClientWrapper(httpClient, logger);
        }

        private static IHttpClientWrapper CreateHttpClientInternal(string baseUrl, HttpClientHandlerConfig config, ILogger logger, X509Certificate2 certificate = null)
        {
            var handler = HttpClientHandlerFactory.Create(config);
            if (certificate != null)
            {
                handler.ClientCertificates.Add(certificate);
            }
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(EnsureTrailingSlash(baseUrl))
            };
            return new HttpClientWrapper(httpClient, logger);
        }

        private static string EnsureTrailingSlash(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }
    }
}
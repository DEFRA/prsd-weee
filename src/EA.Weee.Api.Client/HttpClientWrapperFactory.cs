namespace EA.Weee.Api.Client
{
    using System;
    using System.Net.Http;
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

        private static IHttpClientWrapper CreateHttpClientInternal(string baseUrl, HttpClientHandlerConfig config, ILogger logger, X509Certificate2 certificate = null)
        {
            var handler = HttpClientHandlerFactory.Create(config);

            if (certificate != null)
            {
                handler.ClientCertificates.Add(certificate);
            }

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            return new HttpClientWrapper(httpClient, logger);
        }
    }
}
namespace EA.Weee.Api.Client
{
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpClientWrapperFactory : IHttpClientWrapperFactory
    {
        public IHttpClientWrapper CreateHttpClient(string baseUrl, HttpClientHandlerConfig config, X509Certificate2 certificate, ILogger logger)
        {
            var handler = HttpClientHandlerFactory.Create(config);
            handler.ClientCertificates.Add(certificate);
            var baseUri = new Uri(baseUrl);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = baseUri
            };
            return new HttpClientWrapper(httpClient, logger);
        }
    }
}

namespace EA.Weee.Api.Client
{
    using Serilog;
    using System.Security.Cryptography.X509Certificates;

    public interface IHttpClientWrapperFactory
    {
        IHttpClientWrapper CreateHttpClient(string baseUrl, HttpClientHandlerConfig config, ILogger logger);

        IHttpClientWrapper CreateHttpClientWithCertificate(string baseUrl, HttpClientHandlerConfig config,
            ILogger logger, X509Certificate2 certificate);

        IHttpClientWrapper CreateHttpClientWithAuthorization(string baseUrl, HttpClientHandlerConfig config,
            ILogger logger, string scheme, string parameter);
    }
}

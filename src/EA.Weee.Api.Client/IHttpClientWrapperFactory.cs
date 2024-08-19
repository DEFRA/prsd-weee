namespace EA.Weee.Api.Client
{
    using Serilog;
    using System.Security.Cryptography.X509Certificates;

    public interface IHttpClientWrapperFactory
    {
        IHttpClientWrapper CreateHttpClient(string baseUrl, HttpClientHandlerConfig config, ILogger logger);
    }
}

namespace EA.Weee.Api.Client
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHttpClientWrapper : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}

namespace EA.Weee.Api.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClientWrapper : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}

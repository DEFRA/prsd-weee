namespace EA.Weee.Api.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}

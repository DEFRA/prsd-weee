namespace EA.Weee.Api.Client.Actions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Entities;
    using Prsd.Core.Web.Extensions;

    public class ErrorLog : IErrorLog
    {
        private const string Controller = "ErrorLog/";
        private readonly HttpClient httpClient;

        public ErrorLog(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> Create(string accessToken, ErrorData errorData)
        {
            httpClient.SetBearerToken(accessToken);
            
            var response = await httpClient.PostAsJsonAsync(Controller + "Create", errorData);
            return response.IsSuccessStatusCode;
        }

        public async Task<ErrorData> Get(string accessToken, string id, string applicationName = "")
        {
            httpClient.SetBearerToken(accessToken);

            var uri = Controller + id;

            if (!string.IsNullOrWhiteSpace(applicationName))
            {
                uri += string.Format("?applicationName={0}", applicationName);
            }

            var response = await httpClient.GetAsync(uri);
            return await response.CreateResponseAsync<ErrorData>();
        }

        public async Task<PagedErrorDataList> GetList(string accessToken, int pageIndex, int pageSize, string applicationName = "")
        {
            httpClient.SetBearerToken(accessToken);

            var uri = Controller + "list" + string.Format("?pageIndex={0}&pageSize={1}", pageIndex, pageSize);

            if (!string.IsNullOrWhiteSpace(applicationName))
            {
                uri += string.Format("&applicationName={0}", applicationName);
            }

            var response = await httpClient.GetAsync(uri);
            return await response.CreateResponseAsync<PagedErrorDataList>();
        }
    }
}
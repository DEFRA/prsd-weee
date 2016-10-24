namespace EA.Weee.Api.Client.Actions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Prsd.Core.Web.Extensions;

    public class SmokeTest : ISmokeTest
    {
        private const string Controller = "SmokeTest/";
        private readonly HttpClient httpClient;

        public SmokeTest(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> PerformTest()
        {
            string url = Controller + "PerformTest";
            var response = await httpClient.GetAsync(url);

            return await response.CreateResponseAsync<bool>();
        }
    }
}

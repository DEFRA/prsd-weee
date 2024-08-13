namespace EA.Weee.Api.Client
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CompaniesHouseClient : ICompaniesHouseClient
    {
        private readonly HttpClient httpClient;

        public CompaniesHouseClient(string baseUrl, HttpClientHandlerConfig config, X509Certificate2 certificate)
        {
            var baseUri = new Uri(baseUrl);
            var handler = HttpClientHandlerFactory.Create(config);

            handler.ClientCertificates.Add(certificate);

            httpClient = new HttpClient(handler)
            {
                BaseAddress = baseUri
            };
        }

        public async Task<T> GetCompanyDetailsAsync<T>(string endpoint, string companyReference)
        {
            var response = await httpClient.GetAsync($"{endpoint}{companyReference}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return System.Text.Json.JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions());
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}

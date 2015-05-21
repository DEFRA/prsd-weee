namespace EA.Weee.Api.Client.Actions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Entities;
    using Prsd.Core.Web.Extensions;

    internal class Registration : IRegistration
    {
        private const string controller = "Registration/";
        private readonly HttpClient httpClient;

        public Registration(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> RegisterApplicantAsync(ApplicantRegistrationData applicantRegistrationData)
        {
            var response = await httpClient.PostAsJsonAsync(controller + "Register", applicantRegistrationData);
            return await response.CreateResponseAsync<string>();
        }
    }
}
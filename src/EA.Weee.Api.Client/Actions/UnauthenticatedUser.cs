namespace EA.Weee.Api.Client.Actions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Entities;
    using Prsd.Core.Web.Extensions;

    internal class UnauthenticatedUser : IUnauthenticatedUser
    {
        private const string Controller = "UnauthenticatedUser/";
        private readonly HttpClient httpClient;

        public UnauthenticatedUser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> CreateUserAsync(UserCreationData userCreationData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "CreateUser", userCreationData);
            return await response.CreateResponseAsync<string>();
        }

        public async Task<bool> ActivateUserAccountEmailAsync(ActivatedUserAccountData activatedUserAccountData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "ActivateUserAccount", activatedUserAccountData);

            return await response.CreateResponseAsync<bool>();
        }

        public async Task<string> GetUserAccountActivationTokenAsync(string accessToken)
        {
            httpClient.SetBearerToken(accessToken);

            string url = Controller + "GetUserAccountActivationToken";
            var response = await httpClient.GetAsync(url);

            return await response.CreateResponseAsync<string>();
        }
    }
}
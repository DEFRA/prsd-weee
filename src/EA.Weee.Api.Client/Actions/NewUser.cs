namespace EA.Weee.Api.Client.Actions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Entities;
    using Prsd.Core.Web.Extensions;

    internal class NewUser : INewUser
    {
        private const string Controller = "NewUser/";
        private readonly HttpClient httpClient;

        public NewUser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> CreateUserAsync(UserCreationData userCreationData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "CreateUser", userCreationData);
            return await response.CreateResponseAsync<string>();
        }

        public async Task<bool> VerifyEmailAsync(VerifiedEmailData verifiedEmailData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "VerifyEmail", verifiedEmailData);

            return await response.CreateResponseAsync<bool>();
        }

        public async Task<string> GetUserEmailVerificationTokenAsync(string accessToken)
        {
            httpClient.SetBearerToken(accessToken);

            string url = Controller + "GetUserEmailVerificationToken";
            var response = await httpClient.GetAsync(url);

            return await response.CreateResponseAsync<string>();
        }
    }
}
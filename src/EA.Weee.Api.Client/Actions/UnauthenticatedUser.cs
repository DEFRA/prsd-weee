namespace EA.Weee.Api.Client.Actions
{
    using Entities;
    using Prsd.Core.Web.Extensions;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class UnauthenticatedUser : IUnauthenticatedUser
    {
        private const string Controller = "UnauthenticatedUser/";
        private readonly HttpClient httpClient;

        public UnauthenticatedUser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> CreateInternalUserAsync(InternalUserCreationData userCreationData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "CreateInternalUser", userCreationData);
            return await response.CreateResponseAsync<string>();
        }

        public async Task<string> CreateExternalUserAsync(ExternalUserCreationData userCreationData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "CreateExternalUser", userCreationData);
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

        public async Task<bool> ResendActivationEmail(string accessToken, string activationBaseUrl)
        {
            httpClient.SetBearerToken(accessToken);

            string url = Controller + "ResendActivationEmail";
            
            ResendActivationEmailRequest model = new ResendActivationEmailRequest()
            {
                ActivationBaseUrl = activationBaseUrl,
            };

            var response = await httpClient.PostAsJsonAsync(url, model);

            return await response.CreateResponseAsync<bool>();
        }

        public async Task<bool> ResendActivationEmailByUserId(string userId, string emailAddress, string activationBaseUrl)
        {
            string url = Controller + "ResendActivationEmailByUserId";

            ResendActivationEmailByUserIdRequest model = new ResendActivationEmailByUserIdRequest()
            {
                UserId = userId,
                EmailAddress = emailAddress,
                ActivationBaseUrl = activationBaseUrl,
            };

            var response = await httpClient.PostAsJsonAsync(url, model);

            return await response.CreateResponseAsync<bool>();
        }

        public async Task<bool> ResetPasswordAsync(PasswordResetData passwordResetData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "ResetPassword", passwordResetData);

            return await response.CreateResponseAsync<bool>();
        }

        public async Task<PasswordResetRequestResult> ResetPasswordRequestAsync(PasswordResetRequest passwordResetRequest)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "ResetPasswordRequest", passwordResetRequest);

            return await response.CreateResponseAsync<PasswordResetRequestResult>();
        }

        public async Task<bool> IsPasswordResetTokenValidAsync(PasswordResetData passwordResetData)
        {
            var response = await httpClient.PostAsJsonAsync(Controller + "IsPasswordResetTokenValid", passwordResetData);

            return await response.CreateResponseAsync<bool>();
        }
    }
}
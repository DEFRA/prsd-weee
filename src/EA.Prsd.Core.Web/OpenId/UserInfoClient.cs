namespace EA.Prsd.Core.Web.OpenId
{
    using Extensions;
    using System;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class UserInfoClient : IUserInfoClient
    {
        private readonly string baseUrl;

        public UserInfoClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var userInfoClient = new IdentityModel.Client.UserInfoClient(
                new Uri(baseUrl.EnsureTrailingSlash() + "connect/userinfo"), accessToken);

            return await userInfoClient.GetAsync();
        }
    }
}
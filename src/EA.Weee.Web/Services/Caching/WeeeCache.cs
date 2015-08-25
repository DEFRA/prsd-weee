namespace EA.Weee.Web.Services.Caching
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Users;
    using Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web;

    public class WeeeCache : IWeeeCache
    {
        private readonly ICacheProvider provider;
        private readonly Func<IWeeeClient> apiClient;

        public Cache<Guid, string> UserNames { get; private set; }
        public Cache<Guid, string> OrganisationNames { get; private set; }
        public Cache<Guid, string> SchemeNames { get; private set; }

        public WeeeCache(ICacheProvider provider, Func<IWeeeClient> apiClient)
        {
            this.provider = provider;
            this.apiClient = apiClient;

            UserNames = new Cache<Guid, string>(
                provider,
                "UserName",
                TimeSpan.FromMinutes(5),
                (key) => key.ToString(),
                (key) => FetchUserNameFromApi(key));

            OrganisationNames = new Cache<Guid, string>(
                provider,
                "OrganisationName",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchOrganisationNameFromApi(key));

            SchemeNames = new Cache<Guid, string>(
                provider,
                "OrganisationName",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchSchemeNameFromApi(key));
        }

        private async Task<string> FetchUserNameFromApi(Guid userId)
        {
            using (var client = apiClient())
            {
                string accessToken = HttpContext.Current.User.GetAccessToken();

                var request = new GetUserData(userId.ToString());
                var result = await client.SendAsync(accessToken, request);
                
                return string.Format("{0} {1}", result.FirstName, result.Surname).Trim();
            }
        }

        private async Task<string> FetchOrganisationNameFromApi(Guid organisationId)
        {
            using (var client = apiClient())
            {
                string accessToken = HttpContext.Current.User.GetAccessToken();

                var request = new GetOrganisationInfo(organisationId);
                var result = await client.SendAsync(accessToken, request);

                return result.Name;
            }
        }

        private async Task<string> FetchSchemeNameFromApi(Guid schemeId)
        {
            using (var client = apiClient())
            {
                string accessToken = HttpContext.Current.User.GetAccessToken();

                var request = new GetSchemeById(schemeId);
                var result = await client.SendAsync(accessToken, request);

                return result.Name;
            }
        }

        public Task<string> FetchUserName(Guid userId)
        {
            return UserNames.Fetch(userId);
        }

        public Task<string> FetchOrganisationName(Guid organisationId)
        {
            return OrganisationNames.Fetch(organisationId);
        }

        public Task<string> FetchSchemeName(Guid schemeId)
        {
            return SchemeNames.Fetch(schemeId);
        }
    }
}
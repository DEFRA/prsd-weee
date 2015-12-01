namespace EA.Weee.Web.Services.Caching
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Search;
    using EA.Weee.Requests.Users;
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class WeeeCache : IWeeeCache
    {
        private readonly ICacheProvider provider;
        private readonly Func<IWeeeClient> apiClient;

        public Cache<Guid, string> UserNames { get; private set; }
        public Cache<Guid, string> OrganisationNames { get; private set; }
        public Cache<Guid, string> SchemeNames { get; private set; }
        public Cache<Guid, int> UserActiveCompleteOrganisationCount { get; private set; }
        public Cache<Guid, SchemePublicInfo> SchemePublicInfos { get; private set; }
        public SingleItemCache<IList<ProducerSearchResult>> ProducerSearchResultList { get; private set; }
        public SingleItemCache<IList<OrganisationSearchResult>> OrganisationSearchResultList { get; private set; }

        private string accessToken;

        public WeeeCache(ICacheProvider provider, Func<IWeeeClient> apiClient)
        {
            this.provider = provider;
            this.apiClient = apiClient;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                accessToken = HttpContext.Current.User.GetAccessToken();
            }

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
                "SchemeName",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchSchemeNameFromApi(key));

            UserActiveCompleteOrganisationCount = new Cache<Guid, int>(
                provider,
                "UserActiveCompleteOrganisationCount",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchUserActiveCompleteOrganisationCountFromApi(key));

            SchemePublicInfos = new Cache<Guid, SchemePublicInfo>(
                provider,
                "SchemeInfos",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchSchemePublicInfoFromApi(key));

            ProducerSearchResultList = new SingleItemCache<IList<ProducerSearchResult>>(
                provider,
                "ProducerPublicInfoList",
                TimeSpan.FromDays(1),
                () => FetchProducerSearchResultListFromApi());

            OrganisationSearchResultList = new SingleItemCache<IList<OrganisationSearchResult>>(
                provider,
                "OrganisationPublicInfoList",
                TimeSpan.FromMinutes(15),
                () => FetchOrganisationSearchResultListFromApi());
        }

        private async Task<string> FetchUserNameFromApi(Guid userId)
        {
            using (var client = apiClient())
            {
                var request = new EA.Weee.Requests.Users.GetUserData(userId.ToString());
                var result = await client.SendAsync(accessToken, request);
                
                return string.Format("{0} {1}", result.FirstName, result.Surname).Trim();
            }
        }

        private async Task<string> FetchOrganisationNameFromApi(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var request = new GetOrganisationInfo(organisationId);
                var result = await client.SendAsync(accessToken, request);

                return result.OrganisationName;
            }
        }

        private async Task<string> FetchSchemeNameFromApi(Guid schemeId)
        {
            using (var client = apiClient())
            {
                var request = new GetSchemeById(schemeId);
                var result = await client.SendAsync(accessToken, request);

                return result.Name;
            }
        }

        private async Task<int> FetchUserActiveCompleteOrganisationCountFromApi(Guid key)
        {
            using (var client = apiClient())
            {
                var request = new GetUserOrganisationsByStatus(new int[0]);
                var result = await client.SendAsync(accessToken, request);

                return result
                    .Where(o => o.UserStatus == Core.Shared.UserStatus.Active)
                    .Where(o => o.Organisation.OrganisationStatus == Core.Shared.OrganisationStatus.Complete)
                    .Count();
            }
        }

        private async Task<SchemePublicInfo> FetchSchemePublicInfoFromApi(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var request = new GetSchemePublicInfo(organisationId);
                var result = await client.SendAsync(accessToken, request);

                return result;
            }
        }

        private async Task<IList<ProducerSearchResult>> FetchProducerSearchResultListFromApi()
        {
            using (var client = apiClient())
            {
                var request = new FetchProducerSearchResultsForCache();
                var result = await client.SendAsync(accessToken, request);

                return result;
            }
        }

        private async Task<IList<OrganisationSearchResult>> FetchOrganisationSearchResultListFromApi()
        {
            using (var client = apiClient())
            {
                var request = new FetchOrganisationSearchResultsForCache();
                var result = await client.SendAsync(accessToken, request);

                return result;
            }
        }

        public Task<string> FetchOrganisationName(Guid organisationId)
        {
            return OrganisationNames.Fetch(organisationId);
        }

        public Task<string> FetchSchemeName(Guid schemeId)
        {
            return SchemeNames.Fetch(schemeId);
        }

        public Task<int> FetchUserActiveCompleteOrganisationCount(Guid userId)
        {
            return UserActiveCompleteOrganisationCount.Fetch(userId);
        }

        public Task<SchemePublicInfo> FetchSchemePublicInfo(Guid organisationId)
        {
            return SchemePublicInfos.Fetch(organisationId);
        }

        public Task<IList<ProducerSearchResult>> FetchProducerSearchResultList()
        {
            return ProducerSearchResultList.Fetch();
        }

        Task<IList<ProducerSearchResult>> ISearchResultProvider<ProducerSearchResult>.FetchAll()
        {
            return FetchProducerSearchResultList();
        }

        public async Task InvalidateProducerSearch()
        {
            await ProducerSearchResultList.InvalidateCache();
        }

        public Task<IList<OrganisationSearchResult>> FetchOrganisationSearchResultList()
        {
            return OrganisationSearchResultList.Fetch();
        }

        Task<IList<OrganisationSearchResult>> ISearchResultProvider<OrganisationSearchResult>.FetchAll()
        {
            return FetchOrganisationSearchResultList();
        }

        public async Task InvalidateOrganisationSearch()
        {
            await OrganisationSearchResultList.InvalidateCache();
        }
    }
}
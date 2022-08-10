namespace EA.Weee.Web.Services.Caching
{
    using Core.AatfReturn;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Search;
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Shared;

    public class WeeeCache : IWeeeCache
    {
        private readonly ICacheProvider provider;
        private readonly ConfigurationService configurationService;
        private readonly Func<IWeeeClient> apiClient;

        public Cache<Guid, string> UserNames { get; private set; }
        public Cache<Guid, string> OrganisationNames { get; private set; }
        public Cache<Guid, string> SchemeNames { get; private set; }
        public Cache<Guid, int> UserActiveCompleteOrganisationCount { get; private set; }
        public Cache<Guid, SchemePublicInfo> SchemePublicInfos { get; private set; }
        public Cache<Guid, IList<AatfData>> AatfPublicInfo { get; private set; }
        public Cache<Guid, SchemePublicInfo> SchemePublicInfosBySchemeId { get; private set; }
        public Cache<Guid, IList<ObligatedCategoryValue>> CategoryValues { get; private set; }
        public SingleItemCache<IList<ProducerSearchResult>> ProducerSearchResultList { get; private set; }
        public SingleItemCache<IList<OrganisationSearchResult>> OrganisationSearchResultList { get; private set; }

        public SingleItemCache<DateTime> CurrentDate { get; private set; }

        private readonly string accessToken;

        public WeeeCache(ICacheProvider provider, Func<IWeeeClient> apiClient, ConfigurationService configService)
        {
            this.provider = provider;
            this.apiClient = apiClient;
            this.configurationService = configService;

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
                "AatfName",
                TimeSpan.FromMinutes(configurationService.CurrentConfiguration.OrganisationCacheDurationMins),
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
                (key) => FetchUserActiveCompleteOrganisationCountFromApi());

            SchemePublicInfos = new Cache<Guid, SchemePublicInfo>(
                provider,
                "SchemeInfos",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchSchemePublicInfoFromApi(key));

            SchemePublicInfosBySchemeId = new Cache<Guid, SchemePublicInfo>(
                provider,
                "SchemeInfos",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                (key) => FetchSchemePublicInfoByIdFromApi(key));

            ProducerSearchResultList = new SingleItemCache<IList<ProducerSearchResult>>(
                provider,
                "ProducerPublicInfoList",
                TimeSpan.FromDays(1),
                () => FetchProducerSearchResultListFromApi());

            OrganisationSearchResultList = new SingleItemCache<IList<OrganisationSearchResult>>(
                provider,
                "OrganisationPublicInfoList",
                TimeSpan.FromMinutes(configurationService.CurrentConfiguration.OrganisationCacheDurationMins),
                () => FetchOrganisationSearchResultListFromApi());

            AatfPublicInfo = new Cache<Guid, IList<AatfData>>(
                provider,
                "AatfInfo",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                FetchAatfInfoFromApi);

            CurrentDate = new SingleItemCache<DateTime>(
                provider,
                "CurrentUtcSystemDate",
                (DateTime.Today.AddDays(1).Subtract(DateTime.Now)),
                FetchCurrentUtcDateTime);
        }

        private async Task<string> FetchUserNameFromApi(Guid userId)
        {
            using (var client = apiClient())
            {
                var request = new EA.Weee.Requests.Users.GetUserData(userId.ToString());
                var result = await client.SendAsync(accessToken, request);

                return $"{result.FirstName} {result.Surname}".Trim();
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
                var request = new GetSchemePublicInfoBySchemeId(schemeId);
                var result = await client.SendAsync(accessToken, request);

                return result.Name;
            }
        }

        private async Task<int> FetchUserActiveCompleteOrganisationCountFromApi()
        {
            using (var client = apiClient())
            {
                var request = new GetUserOrganisationsByStatus(new int[0]);
                var result = await client.SendAsync(accessToken, request);

                return result
                    .Where(o => o.UserStatus == UserStatus.Active)
                    .Count(o => o.Organisation.OrganisationStatus == OrganisationStatus.Complete);
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

        private async Task<SchemePublicInfo> FetchSchemePublicInfoByIdFromApi(Guid schemeId)
        {
            using (var client = apiClient())
            {
                var request = new GetSchemePublicInfoBySchemeId(schemeId);
                var result = await client.SendAsync(accessToken, request);

                return result;
            }
        }

        private async Task<IList<AatfData>> FetchAatfInfoFromApi(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var request = new GetAatfByOrganisation(organisationId);
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

        private async Task<DateTime> FetchCurrentUtcDateTime()
        {
            using (var client = apiClient())
            {
                var request = new GetApiUtcDate();
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

        public Task<SchemePublicInfo> FetchSchemePublicInfoBySchemeId(Guid schemeId)
        {
            return SchemePublicInfosBySchemeId.Fetch(schemeId);
        }

        public Task<IList<ProducerSearchResult>> FetchProducerSearchResultList()
        {
            return ProducerSearchResultList.Fetch();
        }

        Task<IList<ProducerSearchResult>> ISearchResultProvider<ProducerSearchResult>.FetchAll()
        {
            return FetchProducerSearchResultList();
        }

        public Task<DateTime> FetchCurrentDate()
        {
            return CurrentDate.Fetch();
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

        public async Task InvalidateAatfCache(Guid id)
        {
            await AatfPublicInfo.InvalidateCache(id);
        }

        public async Task InvalidateCurrentDate()
        {
            await CurrentDate.InvalidateCache();
        }

        public async Task<AatfData> FetchAatfData(Guid organisationId, Guid aatfId)
        {
            var aatfInfo = await AatfPublicInfo.Fetch(organisationId);

            return aatfInfo.FirstOrDefault(a => a.Id == aatfId);
        }

        public async Task InvalidateSchemeCache(Guid id)
        {
            await SchemeNames.InvalidateCache(id);
        }

        public async Task InvalidateSchemePublicInfoCache(Guid organisationId)
        {
            await SchemePublicInfos.InvalidateCache(organisationId);
        }
    }
}
﻿namespace EA.Weee.Web.Services.Caching
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
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using System.Web;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Shared;

    public class WeeeCache : IWeeeCache
    {
        private readonly ICacheProvider provider;
        private readonly ConfigurationService configurationService;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IHttpContextService httpContextService;

        public Cache<Guid, string> UserNames { get; private set; }
        public Cache<Guid, string> OrganisationNames { get; private set; }
        public Cache<Guid, string> SchemeNames { get; private set; }
        public Cache<Guid, int> UserActiveCompleteOrganisationCount { get; private set; }
        public Cache<Guid, SchemePublicInfo> SchemePublicInfos { get; private set; }
        public Cache<Guid, IList<AatfData>> AatfPublicInfo { get; private set; }
        public Cache<Guid, SchemePublicInfo> SchemePublicInfosBySchemeId { get; private set; }

        public Cache<Guid, List<AatfData>> OrganisationAatfDetails { get; private set; }

        public Cache<Guid, IList<ObligatedCategoryValue>> CategoryValues { get; private set; }

        public SingleItemCache<IList<ProducerSearchResult>> ProducerSearchResultList { get; private set; }
        public SingleItemCache<IList<OrganisationSearchResult>> OrganisationSearchResultList { get; private set; }

        public SingleItemCache<IList<SmallProducerSearchResult>> SmallProducerSearchResultList { get; private set; }

        public SingleItemCache<DateTime> CurrentDate { get; private set; }

        private readonly string accessToken;

        public WeeeCache(ICacheProvider provider, Func<IWeeeClient> apiClient, ConfigurationService configService, IHttpContextService httpContextService)
        {
            this.provider = provider;
            this.apiClient = apiClient;
            this.configurationService = configService;
            this.httpContextService = httpContextService;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                accessToken = HttpContext.Current.User.GetAccessToken();
            }

            UserNames = new Cache<Guid, string>(
                provider,
                "UserName",
                TimeSpan.FromMinutes(5),
                (key) => key.ToString(),
                FetchUserNameFromApi);

            OrganisationNames = new Cache<Guid, string>(
                provider,
                "AatfName",
                TimeSpan.FromMinutes(configurationService.CurrentConfiguration.OrganisationCacheDurationMins),
                (key) => key.ToString(),
                FetchOrganisationNameFromApi);

            SchemeNames = new Cache<Guid, string>(
                provider,
                "SchemeName",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                FetchSchemeNameFromApi);

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
                FetchSchemePublicInfoFromApi);

            SchemePublicInfosBySchemeId = new Cache<Guid, SchemePublicInfo>(
                provider,
                "SchemeInfos",
                TimeSpan.FromMinutes(15),
                (key) => key.ToString(),
                FetchSchemePublicInfoByIdFromApi);

            ProducerSearchResultList = new SingleItemCache<IList<ProducerSearchResult>>(
                provider,
                "ProducerPublicInfoList",
                TimeSpan.FromDays(1),
                FetchProducerSearchResultListFromApi);

            SmallProducerSearchResultList = new SingleItemCache<IList<SmallProducerSearchResult>>(
                provider,
                "SmallProducerPublicInfoList",
                TimeSpan.FromDays(1),
                FetchSmallProducerSearchResultListFromApi);

            OrganisationSearchResultList = new SingleItemCache<IList<OrganisationSearchResult>>(
                provider,
                "OrganisationPublicInfoList",
                TimeSpan.FromMinutes(configurationService.CurrentConfiguration.OrganisationCacheDurationMins),
                FetchOrganisationSearchResultListFromApi);

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

            OrganisationAatfDetails = new Cache<Guid, List<AatfData>>(
                provider,
                "OrganisationAatfData",
                TimeSpan.FromDays(1),
                (organisationId => organisationId.ToString()),
                FetchAatfsForOrganisation);
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

        private async Task<IList<SmallProducerSearchResult>> FetchSmallProducerSearchResultListFromApi()
        {
            using (var client = apiClient())
            {
                var request = new FetchSmallProducerSearchResultsForCache();
                var result = await client.SendAsync(accessToken, request);

                return result;
            }
        }

        private async Task<IList<OrganisationSearchResult>> FetchOrganisationSearchResultListFromApi()
        {
            using (var client = apiClient())
            {
                var request = new FetchOrganisationSearchResultsForCache(configurationService.CurrentConfiguration.SmallProducerFeatureEnabledFrom);
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

        private async Task<List<AatfData>> FetchAatfsForOrganisation(Guid organisationId)
        {
            using (var client = apiClient())
            {
                var request = new GetAatfByOrganisation(organisationId);
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

        public Task<IList<SmallProducerSearchResult>> FetchSmallProducerSearchResultList()
        {
            return SmallProducerSearchResultList.Fetch();
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

        Task<IList<SmallProducerSearchResult>> ISearchResultProvider<SmallProducerSearchResult>.FetchAll()
        {
            return FetchSmallProducerSearchResultList();
        }

        public async Task InvalidateOrganisationSearch()
        {
            await OrganisationSearchResultList.InvalidateCache();
        }

        public async Task InvalidateAatfCache(Guid id)
        {
            await AatfPublicInfo.InvalidateCache(id);
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

        public async Task<List<AatfData>> FetchAatfDataForOrganisationData(Guid organisationId)
        {
            var hasAccess = httpContextService.HasOrganisationClaim(organisationId);
            if (!hasAccess)
            {
                throw new AuthenticationException($"User does not have access to organisation cache {organisationId}");
            }

            return await OrganisationAatfDetails.Fetch(organisationId);
        }

        public async Task InvalidateAatfDataForOrganisationDataCache(Guid organisationId)
        {
            await OrganisationAatfDetails.InvalidateCache(organisationId);
        }

        public async Task InvalidateOrganisationNameCache(Guid organisationId)
        {
            await OrganisationNames.InvalidateCache(organisationId);
        }

        public async Task InvalidateSmallProducerSearch()
        {
            await SmallProducerSearchResultList.InvalidateCache();
        }

        public void Clear()
        {
            provider.ClearCache();
        }
    }
}
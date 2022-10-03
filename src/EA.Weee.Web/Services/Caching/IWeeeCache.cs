﻿namespace EA.Weee.Web.Services.Caching
{
    using Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWeeeCache : ISearchResultProvider<ProducerSearchResult>, ISearchResultProvider<OrganisationSearchResult>
    {
        Task<string> FetchOrganisationName(Guid organisationId);

        Task<string> FetchSchemeName(Guid schemeId);

        Task<int> FetchUserActiveCompleteOrganisationCount(Guid userId);

        Task<SchemePublicInfo> FetchSchemePublicInfo(Guid organisationId);

        Task<SchemePublicInfo> FetchSchemePublicInfoBySchemeId(Guid schemeId);

        Task<IList<ProducerSearchResult>> FetchProducerSearchResultList();

        Task<IList<OrganisationSearchResult>> FetchOrganisationSearchResultList();

        Task InvalidateProducerSearch();

        Task InvalidateOrganisationSearch();

        Task InvalidateAatfCache(Guid id);

        Task InvalidateSchemeCache(Guid id);

        Task<AatfData> FetchAatfData(Guid organisationId, Guid aatfId);

        Task InvalidateSchemePublicInfoCache(Guid organisationId);

        Task<List<AatfData>> FetchAatfDataForOrganisationData(Guid organisationId);

        Task InvalidateAatfDataForOrganisationDataCache(Guid organisationId);

        Task InvalidateOrganisationNameCache(Guid organisationId);
        void Clear();
    }
}

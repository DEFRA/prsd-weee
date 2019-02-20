namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Search;

    public interface IWeeeCache : ISearchResultProvider<ProducerSearchResult>, ISearchResultProvider<OrganisationSearchResult>
    {
        Task<string> FetchOrganisationName(Guid organisationId);

        Task<string> FetchSchemeName(Guid schemeId);

        Task<int> FetchUserActiveCompleteOrganisationCount(Guid userId);

        Task<SchemePublicInfo> FetchSchemePublicInfo(Guid organisationId);

        Task<IList<ProducerSearchResult>> FetchProducerSearchResultList();

        Task<IList<OrganisationSearchResult>> FetchOrganisationSearchResultList();

        Task InvalidateProducerSearch();

        Task InvalidateOrganisationSearch();

        Task<AatfData> FetchAatfData(Guid organisationId, Guid aatfId);
    }
}

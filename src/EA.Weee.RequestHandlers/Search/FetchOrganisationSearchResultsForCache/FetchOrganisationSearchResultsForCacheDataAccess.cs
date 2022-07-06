namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class FetchOrganisationSearchResultsForCacheDataAccess : IFetchOrganisationSearchResultsForCacheDataAccess
    {
        private readonly WeeeContext context;
        private readonly IMap<Address, AddressData> addressMapper;

        public FetchOrganisationSearchResultsForCacheDataAccess(WeeeContext context, IMap<Address, AddressData> addressMapper)
        {
            this.context = context;
            this.addressMapper = addressMapper;
        }

        public async Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations()
        {
            var organisations = await context.Organisations
                .Where(p => p.OrganisationStatus.Value == Domain.Organisation.OrganisationStatus.Complete.Value)
                .ToListAsync();

            var schemes = await context.Schemes.ToListAsync();
            var aatfs = await context.Aatfs.ToListAsync();

            return organisations.Select(r => new OrganisationSearchResult()
            {
                OrganisationId = r.Id,
                Name = r.OrganisationName,
                Address = addressMapper.Map(r.BusinessAddress),
                PcsCount = schemes.Count(p => p.OrganisationId == r.Id && p.SchemeStatus != Domain.Scheme.SchemeStatus.Rejected),
                AatfCount = aatfs.Count(p => p.Organisation.Id == r.Id && p.FacilityType == FacilityType.Aatf),
                AeCount = aatfs.Count(p => p.Organisation.Id == r.Id && p.FacilityType == FacilityType.Ae),
                IsBalancingScheme = r.ProducerBalancingScheme != null
            })
                .Where(r => r.PcsCount > 0 || r.AatfCount > 0 || r.AeCount > 0 || r.IsBalancingScheme)
                .OrderBy(r => r.Name)
                .ToList();
        }
    }
}

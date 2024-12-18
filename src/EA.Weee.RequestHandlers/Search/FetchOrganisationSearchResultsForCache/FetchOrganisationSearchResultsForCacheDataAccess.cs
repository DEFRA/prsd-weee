namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class FetchOrganisationSearchResultsForCacheDataAccess : IFetchOrganisationSearchResultsForCacheDataAccess
    {
        private readonly WeeeContext context;
        private readonly IMap<Address, AddressData> addressMapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public FetchOrganisationSearchResultsForCacheDataAccess(WeeeContext context, IMap<Address, AddressData> addressMapper, ISystemDataDataAccess systemDataDataAccess)
        {
            this.context = context;
            this.addressMapper = addressMapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations(DateTime smallProducerEnabledFrom)
        {
            var currentDate = await systemDataDataAccess.GetSystemDateTime();

            var completeOrganisations = await context.Organisations
                .Where(p => p.OrganisationStatus.Value == Domain.Organisation.OrganisationStatus.Complete.Value && (!p.NpwdMigrated || (p.NpwdMigrated && currentDate >= smallProducerEnabledFrom)))
                .Include(o => o.BusinessAddress)
                .Include(o => o.ProducerBalancingScheme)
                .Select(o => new
                {
                    Organisation = o,
                    PcsCount = context.Schemes.Count(s => s.OrganisationId == o.Id && s.SchemeStatus.Value != Domain.Scheme.SchemeStatus.Rejected.Value),
                    AatfCount = context.Aatfs.Count(a => a.Organisation.Id == o.Id && a.FacilityType.Value == FacilityType.Aatf.Value),
                    AeCount = context.Aatfs.Count(a => a.Organisation.Id == o.Id && a.FacilityType.Value == FacilityType.Ae.Value),
                    DirectRegistrantCount = context.DirectRegistrants.Count(d => d.Organisation.Id == o.Id)
                })
                .ToListAsync();

            return completeOrganisations
                .Select(r => new OrganisationSearchResult
                {
                    OrganisationId = r.Organisation.Id,
                    Name = r.Organisation.OrganisationName,
                    Address = addressMapper.Map(r.Organisation.BusinessAddress),
                    PcsCount = r.PcsCount,
                    AatfCount = r.AatfCount,
                    AeCount = r.AeCount,
                    IsBalancingScheme = r.Organisation.ProducerBalancingScheme != null,
                    NpwdMigrated = r.Organisation.NpwdMigrated,
                    NpwdMigratedComplete = r.Organisation.NpwdMigratedComplete,
                    DirectRegistrantCount = r.DirectRegistrantCount,
                    CompanyRegistrationNumber = r.Organisation.CompanyRegistrationNumber
                })
                .Where(r => r.PcsCount > 0 || r.AatfCount > 0 || r.AeCount > 0 || r.IsBalancingScheme || r.DirectRegistrantCount > 0)
                .OrderBy(r => r.Name)
                .ToList();
        }
    }
}
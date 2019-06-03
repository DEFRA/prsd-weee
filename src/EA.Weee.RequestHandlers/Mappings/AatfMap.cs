namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class AatfMap : IMap<Aatf, AatfData>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;
        private readonly IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap;
        private readonly IMap<AatfAddress, AatfAddressData> aatfAddressMap;
        private readonly IMap<AatfContact, AatfContactData> contactMap;
        private readonly IMap<Organisation, OrganisationData> organisationMap;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityMap;

        public AatfMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap,
            IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap,
            IMap<AatfAddress, AatfAddressData> aatfAddressMap,
            IMap<AatfContact, AatfContactData> contactMap,
            IMap<Organisation, OrganisationData> organisationMap,
            IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
            this.aatfSizeMap = aatfSizeMap;
            this.aatfAddressMap = aatfAddressMap;
            this.contactMap = contactMap;
            this.organisationMap = organisationMap;
            this.facilityMap = facilityMap;
        }

        public AatfData Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var competentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            var aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            var aatfSize = aatfSizeMap.Map(source.Size);

            var address = aatfAddressMap.Map(source.SiteAddress);

            var contact = contactMap.Map(source.Contact);

            var organisation = organisationMap.Map(source.Organisation);

            Core.AatfReturn.FacilityType facilityType = facilityMap.Map(source.FacilityType);

            return new AatfData(source.Id, source.Name, source.ApprovalNumber, source.ComplianceYear, competentAuthority, aatfStatus, address, aatfSize, source.ApprovalDate.GetValueOrDefault())
            {
                Contact = contact,
                Organisation = organisation,
                FacilityType = facilityType
            };
        }
    }
}

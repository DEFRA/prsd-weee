namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using Core.Organisations;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class AatfDataListMap : IMap<Aatf, AatfDataList>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityTypeMap;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public AatfDataListMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap,
            IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityTypeMap,
            IMap<Organisation, OrganisationData> organisationMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
            this.facilityTypeMap = facilityTypeMap;
            this.organisationMap = organisationMap;
        }

        public AatfDataList Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var competentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            var aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            var facilityType = facilityTypeMap.Map(source.FacilityType);

            var organisation = organisationMap.Map(source.Organisation);

            return new AatfDataList(source.Id, source.Name, competentAuthority, source.ApprovalNumber, aatfStatus, organisation, facilityType);
        }
    }
}

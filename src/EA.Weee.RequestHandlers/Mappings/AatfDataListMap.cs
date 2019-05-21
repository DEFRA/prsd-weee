namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class AatfDataListMap : IMap<Aatf, AatfDataList>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityTypeMap;

        public AatfDataListMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap,
            IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> facilityTypeMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
            this.facilityTypeMap = facilityTypeMap;
        }

        public AatfDataList Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            UKCompetentAuthorityData compentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            Core.AatfReturn.AatfStatus aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            Core.AatfReturn.FacilityType facilityType = facilityTypeMap.Map(source.FacilityType);

            return new AatfDataList(source.Id, source.Name, compentAuthority, source.ApprovalNumber, aatfStatus, source.Operator, facilityType);
        }
    }
}

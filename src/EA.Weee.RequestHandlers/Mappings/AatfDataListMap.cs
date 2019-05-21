namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using CoreFacilityType = Core.AatfReturn.FacilityType;
    using DomainFacilityType = Domain.AatfReturn.FacilityType;

    public class AatfDataListMap : IMap<Aatf, AatfDataList>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;

        public AatfDataListMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
        }

        public AatfDataList Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            UKCompetentAuthorityData compentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            Core.AatfReturn.AatfStatus aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            CoreFacilityType facilityType = (CoreFacilityType)source.FacilityType.Value;

            return new AatfDataList(source.Id, source.Name, compentAuthority, source.ApprovalNumber, aatfStatus, source.Operator, facilityType);
        }
    }
}

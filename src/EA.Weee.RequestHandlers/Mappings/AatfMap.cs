namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class AatfMap : IMap<Aatf, AatfData>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;
        private readonly IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap;
        private readonly IMap<AatfAddress, AatfAddressData> aatfAddressMap;

        public AatfMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap,
            IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap,
            IMap<AatfAddress, AatfAddressData> aatfAddressMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
            this.aatfSizeMap = aatfSizeMap;
            this.aatfAddressMap = aatfAddressMap;
        }

        public AatfData Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            UKCompetentAuthorityData compentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            Core.AatfReturn.AatfStatus aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            Core.AatfReturn.AatfSize aatfSize = aatfSizeMap.Map(source.Size);

            AatfAddressData address = aatfAddressMap.Map(source.SiteAddress);

            return new AatfData(source.Id, source.Name, source.ApprovalNumber, compentAuthority, aatfStatus, address, aatfSize, source.ApprovalDate);
        }
    }
}

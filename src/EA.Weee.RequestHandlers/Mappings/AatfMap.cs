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

        public AatfMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap, IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
        }

        public AatfData Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            UKCompetentAuthorityData compentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            Core.AatfReturn.AatfStatus aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            return new AatfData(source.Id, source.Name, source.ApprovalNumber, compentAuthority, aatfStatus);
        }
    }
}

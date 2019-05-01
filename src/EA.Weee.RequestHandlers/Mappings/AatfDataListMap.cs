namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    public class AatfDataListMap : IMap<Aatf, AatfDataList>
    {
        public AatfDataList Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new AatfDataList(source.Id, source.Name, source.CompetentAuthority, source.ApprovalNumber, source.AatfStatus, source.Operator);
        }
    }
}

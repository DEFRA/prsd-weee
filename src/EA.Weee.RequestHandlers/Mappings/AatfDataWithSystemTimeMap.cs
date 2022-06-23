namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Prsd.Core.Mapper;

    public class AatfDataWithSystemTimeMap : IMap<AatfWithSystemDateMapperObject, AatfData>
    {
        private readonly IMap<Aatf, AatfData> aatfMap;

        public AatfDataWithSystemTimeMap(IMap<Aatf, AatfData> aatfMap)
        {
            this.aatfMap = aatfMap;
        }

        public AatfData Map(AatfWithSystemDateMapperObject source)
        {
            throw new System.NotImplementedException();
        }
    }
}

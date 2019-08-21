namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using FacilityType = Domain.AatfReturn.FacilityType;

    public class FacilityTypeMap : IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>
    {
        public Core.AatfReturn.FacilityType Map(Domain.AatfReturn.FacilityType source)
        {
            if (source == FacilityType.Aatf)
            {
                return Core.AatfReturn.FacilityType.Aatf;
            }

            return Core.AatfReturn.FacilityType.Ae;
        }
    }
}

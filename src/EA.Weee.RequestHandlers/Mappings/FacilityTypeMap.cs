namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
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

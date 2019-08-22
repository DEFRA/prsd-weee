namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetAatfs : IRequest<List<AatfDataList>>
    {
        public FacilityType FacilityType { get; private set; }

        public AatfFilter Filter { get; private set; }

        public GetAatfs(FacilityType facilityType, AatfFilter filter = null)
        {
            FacilityType = facilityType;
            Filter = filter;
        }
    }
}

namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;
    public class GetAatfs : IRequest<List<AatfDataList>>
    {
        public FacilityType FacilityType { get; private set; }

        public GetAatfs(FacilityType facilityType)
        {
            this.FacilityType = facilityType;
        }
    }
}

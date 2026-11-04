namespace EA.Weee.Requests.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;

    public class GetAatfOrAeRetentionPeriodComplianceYears : IRequest<List<int>>
    {
        public GetAatfOrAeRetentionPeriodComplianceYears(FacilityType facilityType, int tentionPeriod)
        {
            FacilityType = facilityType;
            RetentionPeriod = tentionPeriod;
        }

        public FacilityType FacilityType { get; set; }
        public int RetentionPeriod { get; set; }
    }
}

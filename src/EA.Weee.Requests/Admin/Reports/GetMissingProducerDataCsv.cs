namespace EA.Weee.Requests.Admin.Reports
{
    using System;
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;
    
    public class GetMissingProducerDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }
        public ObligationType ObligationType { get; private set; }
        public int? Quarter { get; private set; }
        public Guid? SchemeId { get; private set; }

        public GetMissingProducerDataCsv(int complianceYear,
            ObligationType obligationType,
            int? quarter,
            Guid? schemeId)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
            Quarter = quarter;
            SchemeId = schemeId;
        }
    }
}

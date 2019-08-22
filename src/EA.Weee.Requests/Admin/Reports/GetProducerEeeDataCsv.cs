namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class GetProducerEeeDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public Guid? SchemeId { get; set; }

        public ObligationType ObligationType { get; private set; }

        public GetProducerEeeDataCsv(int complianceYear, Guid? schemeId, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            SchemeId = schemeId;
            ObligationType = obligationType;
        }
    }
}

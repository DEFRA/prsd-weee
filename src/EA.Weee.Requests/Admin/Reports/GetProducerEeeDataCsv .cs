namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetProducerEeeDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public ObligationType ObligationType { get; private set; }
        public GetProducerEeeDataCsv(int complianceYear, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
        }
    }
}

namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetProducersEEEDataCSV : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public ObligationType ObligationType { get; private set; }
        public GetProducersEEEDataCSV(int complianceYear, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
        }
    }
}

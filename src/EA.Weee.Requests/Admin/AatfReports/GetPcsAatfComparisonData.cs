namespace EA.Weee.Requests.Admin.AatfReports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetPcsAatfComparisonData : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public int? Quarter { get; private set; }

        public string ObligationType { get; private set; }

        public GetPcsAatfComparisonData(int complianceYear, int? quarter,
          string obligationType)
        {
            ComplianceYear = complianceYear;
            Quarter = quarter;
            ObligationType = obligationType;
        }
    }
}

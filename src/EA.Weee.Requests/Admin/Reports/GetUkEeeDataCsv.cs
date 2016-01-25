namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetUkEeeDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public GetUkEeeDataCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

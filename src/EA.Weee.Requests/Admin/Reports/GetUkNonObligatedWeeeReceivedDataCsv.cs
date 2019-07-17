namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetUkNonObligatedWeeeReceivedDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public GetUkNonObligatedWeeeReceivedDataCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

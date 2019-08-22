namespace EA.Weee.Requests.Admin.AatfReports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetUkNonObligatedWeeeReceivedAtAatfsDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public string AatfName { get; private set; }

        public GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(int complianceYear, string aatfName)
        {
            ComplianceYear = complianceYear;
            AatfName = aatfName;
        }
    }
}

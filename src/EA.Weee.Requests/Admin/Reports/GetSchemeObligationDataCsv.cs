namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSchemeObligationDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public GetSchemeObligationDataCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
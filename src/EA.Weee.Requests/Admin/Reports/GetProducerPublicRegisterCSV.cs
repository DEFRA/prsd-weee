namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetProducerPublicRegisterCSV : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public GetProducerPublicRegisterCSV(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

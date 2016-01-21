namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetSchemeWeeeCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public GetSchemeWeeeCsv(int complianceYear, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
        }
    }
}

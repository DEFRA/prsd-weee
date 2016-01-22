namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetUKWeeeCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public GetUKWeeeCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

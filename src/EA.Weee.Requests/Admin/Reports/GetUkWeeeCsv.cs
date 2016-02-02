namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetUkWeeeCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public GetUkWeeeCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

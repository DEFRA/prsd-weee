namespace EA.Weee.Requests.Admin.AatfReports
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

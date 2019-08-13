namespace EA.Weee.Requests.Admin.AatfReports
{
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetUkWeeeAtAatfsCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public GetUkWeeeAtAatfsCsv(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}

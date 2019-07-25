namespace EA.Weee.Requests.Admin.AatfReports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetUkNonObligatedWeeeReceivedAtAatfsDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PatAreaId { get; private set; }

        public string AatfName { get; private set; }

        public GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(int complianceYear, Guid? authority, Guid? patAreaId, string aatfName)
        {
            ComplianceYear = complianceYear;
            AuthorityId = authority;
            PatAreaId = patAreaId;
            AatfName = aatfName;
        }
    }
}

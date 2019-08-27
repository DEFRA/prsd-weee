namespace EA.Weee.Requests.Admin.AatfReports
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;
    public class GetAllAatfSentOnDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public string ObligationType { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public GetAllAatfSentOnDataCsv(int complianceYear,
           string obligationType, Guid? authorityId, Guid? panArea)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
            AuthorityId = authorityId;
            PanArea = panArea;
        }
    }
}

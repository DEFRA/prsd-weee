namespace EA.Weee.Requests.Admin.AatfReports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;
    public class GetAllAatfSentOnDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public string ObligationType { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public string AATFName { get; private set; }

        public GetAllAatfSentOnDataCsv(int complianceYear, 
           string obligationType, string aatfName, Guid? authorityId, Guid? panArea)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
            AATFName = aatfName;
            AuthorityId = authorityId;
            PanArea = panArea;
        }
    }
}

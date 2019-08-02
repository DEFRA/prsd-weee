namespace EA.Weee.Requests.Admin.AatfReports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;
    public class GetAllAatfReuseSitesCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public GetAllAatfReuseSitesCsv(int complianceYear, Guid? authorityId, Guid? panArea)
        {
            ComplianceYear = complianceYear;
            AuthorityId = authorityId;
            PanArea = panArea;
        }
    }
}

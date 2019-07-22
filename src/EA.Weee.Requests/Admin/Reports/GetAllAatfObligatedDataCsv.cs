namespace EA.Weee.Requests.Admin.Reports
{
    using System;
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetAllAatfObligatedDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }
       
        public string ObligationType { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public int ColumnType { get; private set; }

        public string AATFName { get; private set; }

        public GetAllAatfObligatedDataCsv(int complianceYear, int columnType,
           string obligationType, string aatfName, Guid? authorityId, Guid? panArea)
        {
            ComplianceYear = complianceYear;
            ColumnType = columnType;
            ObligationType = obligationType;
            AATFName = aatfName;
            AuthorityId = authorityId;
            PanArea = panArea;
        }
    }
}

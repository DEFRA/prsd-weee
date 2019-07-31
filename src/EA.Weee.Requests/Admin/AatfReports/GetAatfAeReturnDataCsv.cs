namespace EA.Weee.Requests.Admin.AatfReports
{
    using System;
    using Core.AatfReturn;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetAatfAeReturnDataCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public int Quarter { get; private set; }

        public FacilityType FacilityType { get; private set; }

        public int? ReturnStatus { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public Guid? LocalArea { get; private set; }

        public string AatfDataUrl { get; private set; }

        public bool IncludeReSubmissions { get; private set; }

        public GetAatfAeReturnDataCsv(int complianceYear,
          int quarter, 
          FacilityType facilityType, 
          int? returnStatus, 
          Guid? authorityId, 
          Guid? panArea, 
          Guid? localArea, 
          string aatfDataUrl,
          bool includeReSubmissions)
        {
            ComplianceYear = complianceYear;
            Quarter = quarter;
            FacilityType = facilityType;
            ReturnStatus = returnStatus;
            AuthorityId = authorityId;
            PanArea = panArea;
            LocalArea = localArea;
            AatfDataUrl = aatfDataUrl;
            IncludeReSubmissions = includeReSubmissions;
        }
    }
}

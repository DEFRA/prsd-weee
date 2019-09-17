namespace EA.Weee.Requests.Admin.AatfReports
{
    using Core.Admin;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin.AatfReports;
    using Prsd.Core.Mediator;
    using System;
    public class GetAatfAeDetailsCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public ReportFacilityType? FacilityType { get; private set; }

        public Guid? AuthorityId { get; private set; }

        public Guid? PanArea { get; private set; }

        public Guid? LocalArea { get; private set; }

        public bool IsPublicRegister { get; private set; }

        public GetAatfAeDetailsCsv(int complianceYear,
          ReportFacilityType? facilityType,
          Guid? authorityId,
          Guid? panArea,
          Guid? localArea, bool isPublicRegister)
        {
            ComplianceYear = complianceYear;
            FacilityType = facilityType;
            AuthorityId = authorityId;
            PanArea = panArea;
            LocalArea = localArea;
            IsPublicRegister = isPublicRegister;
        }
    }
}

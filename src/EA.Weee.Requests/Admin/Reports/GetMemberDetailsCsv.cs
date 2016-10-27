namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetMemberDetailsCsv : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public Guid? SchemeId { get; private set; }

        public Guid? CompetentAuthorityId { get; private set; }

        public bool IncludeRemovedProducer { get; private set; }

        public bool IncludeBrandNames { get; private set; }

        public GetMemberDetailsCsv(int complianceYear, bool includeRemovedProducer = false,
            Guid? schemeId = null, Guid? competentAuthorityId = null, bool includeBrandNames = false)
        {
            CompetentAuthorityId = competentAuthorityId;
            SchemeId = schemeId;
            ComplianceYear = complianceYear;
            IncludeRemovedProducer = includeRemovedProducer;
            IncludeBrandNames = includeBrandNames;
        }
    }
}

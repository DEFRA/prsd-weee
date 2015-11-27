namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetMemberDetailsCSV : IRequest<CSVFileData>
   {
        public int ComplianceYear { get; private set; }

        public Guid? SchemeId { get; private set; }

        public Guid? CompetentAuthorityId { get; private set; }        

        public GetMemberDetailsCSV(int complianceYear, Guid? schemeId = null, Guid? competentAuthorityId = null)
        {        
            CompetentAuthorityId = competentAuthorityId;
            SchemeId = schemeId;
            ComplianceYear = complianceYear;
        }
    }
}

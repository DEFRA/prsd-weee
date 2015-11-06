﻿namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetMemberDetailsCSV : IRequest<MembersDetailsCSVFileData>
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

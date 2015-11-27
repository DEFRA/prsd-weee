namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetPCSChargesCSV : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }
        
        public Guid? CompetentAuthorityId { get; private set; }

        public GetPCSChargesCSV(int complianceYear, Guid? competentAuthorityId = null)
        {
            CompetentAuthorityId = competentAuthorityId;          
            ComplianceYear = complianceYear;
        }
    }
}
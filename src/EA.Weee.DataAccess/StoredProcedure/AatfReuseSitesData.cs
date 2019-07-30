namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [AATF].[getAllAatfReuseSitesCsvData]
    /// </summary>
    public class AatfReuseSitesData
    {
        public int ComplianceYear { get; set; }

        public string Quarter { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string SubmittedBy { get; set; }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public string OrgName { get; set; }
      
        public string Abbreviation { get; set; }

        public string PanName { get; set; }

        public string LaName { get; set; }

        public string SiteName { get; set; }

        public string SiteAddress { get; set; }
    }
}

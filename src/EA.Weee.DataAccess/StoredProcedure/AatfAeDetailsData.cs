namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [AATF].[getAatfAeDetailsDataCsvData]
    /// </summary>
    public class AatfAeDetailsData
    {
        public int ComplianceYear { get; set; }
        public string AppropriateAuthorityAbbr { get; set; }

        public string PanAreaTeam { get; set; }

        public string EaArea { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string ApprovalNumber { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string Size { get; set; }

        public string Status { get; set; }

        public string ContactName { get; set; }

        public string ContactPosition { get; set; }

        public string ContactAddress { get; set; }

        public string ContactPostcode { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string OrganisationName { get; set; }

        public string OrganisationAddress { get; set; }

        public string OrganisationPostcode { get; set; }
    }
}

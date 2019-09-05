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

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string TownCity { get; set; }

        public string CountyRegion { get; set; }

        public string Country { get; set; }

        public string PostCode { get; set; }

        public string ApprovalNumber { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string ApprovalDateString
        {
            get
            {
                return ApprovalDate.ToShortDateString();
            }
        }

    public string Size { get; set; }

        public string Status { get; set; }

        public string ContactName { get; set; }

        public string ContactPosition { get; set; }

        public string ContactAddress1 { get; set; }

        public string ContactAddress2 { get; set; }

        public string ContactTownCity { get; set; }

        public string ContactCountyRegion { get; set; }

        public string ContactCountry { get; set; }

        public string ContactPostcode { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string OrganisationName { get; set; }

        public string OrganisationAddress1 { get; set; }

        public string OrganisationAddress2 { get; set; }

        public string OrganisationTownCity { get; set; }

        public string OrganisationCountyRegion { get; set; }

        public string OrganisationCountry { get; set; }

        public string OrganisationPostcode { get; set; }

        public string AatfAddress { get; set; }

        public string OperatorName { get; set; }

        public string OperatorTradingName { get; set; }

        public string OperatorAddress { get; set; }
    }
}

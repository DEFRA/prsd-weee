namespace EA.Weee.Web.Controllers
{
    using System.Collections.Generic;

    public class CompanyProfile
    {
        public bool CanFile { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyStatus { get; set; }
        public string CompanyStatusDetail { get; set; }
        public string DateOfCreation { get; set; }
        public string DateOfCessation { get; set; }
        public string Etag { get; set; }
        public bool HasBeenLiquidated { get; set; }
        public bool HasCharges { get; set; }
        public bool HasInsolvencyHistory { get; set; }
        public bool IsCommunityInterestCompany { get; set; }
        public string Jurisdiction { get; set; }
        public string LastFullMembersListDate { get; set; }
        public string PartialDataAvailable { get; set; }
        public RegisteredOfficeAddress RegisteredOfficeAddress { get; set; }
        public List<string> SicCodes { get; set; }
        public string Type { get; set; }
        public bool UndeliverableRegisteredOfficeAddress { get; set; }
    }
}
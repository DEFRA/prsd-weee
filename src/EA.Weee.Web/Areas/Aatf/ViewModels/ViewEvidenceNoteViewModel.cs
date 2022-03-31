namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    public class ViewEvidenceNoteViewModel : EvidenceNoteViewModel
    {
        public bool DisplaySuccessMessage { get; set; }

        public string ReferenceId { get; set; } = "E20";

        public string Status { get; set; } = "Draft";

        public int ComplianceYear { get; set; } = 2022;

        public string TypeOfWaste { get; set; } = "Household";

        public string Protocol { get; set; } = "Actual";

        public string SiteName { get; set; } = "Recyling Team LTD";
        public string SiteCode { get; set; } = "WEE/AB0001YZ/ATF";
        public string SiteAddress { get; set; } = "12 May Lane <br />Hammersmith<br />London<br />SW13 TYH";

        public string OperatorName { get; set; } = "Recycling Team LTD";
        public string OperatorAddress { get; set; } = "12 May Lane <br />Hammersmith <br /> London <br /> SW13 TYH";

        public string RecipientName { get; set; } = "ERP UK LTD";
        public string RecipientAddress { get; set; } = "1 Bart Simpson Avenue <br /> Reading <br /> RG1 ATR";

    }
}
namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel;

    public class EvidenceTonnageValueCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }
        public Guid AatfId { get; set; }
        public Guid EvidenceId { get; set; }
        public string Action { get; set; }
        [DisplayName("Total received (tonnes)")]
        public string[] ReceievedPastedValues { get; set; }
        [DisplayName("Reused as whole appliances (tonnes)")]
        public string[] ReusedPastedValues { get; set; }

        public int ComplianceYear { get; set; }
    }
}
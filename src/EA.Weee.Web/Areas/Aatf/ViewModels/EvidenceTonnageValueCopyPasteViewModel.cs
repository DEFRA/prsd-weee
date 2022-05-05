namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;

    public class EvidenceTonnageValueCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }
        public Guid AatfId { get; set; }
        public Guid EvidenceId { get; set; }
        public string Action { get; set; }
        public string[] ReceievedPastedValues { get; set; }
        public string[] ReusedPastedValues { get; set; }
    }
}
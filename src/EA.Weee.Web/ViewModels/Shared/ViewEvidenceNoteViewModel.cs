namespace EA.Weee.Web.ViewModels.Shared
{
    using System.ComponentModel;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Extensions;

    public class ViewEvidenceNoteViewModel : EvidenceNoteViewModel
    {
        public string SuccessMessage { get; set; }

        public bool DisplayMessage => !string.IsNullOrWhiteSpace(SuccessMessage);

        [DisplayName("Actual or protocol")]
        public string ProtocolDisplay => ProtocolValue.HasValue ? ProtocolValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Type of waste")]
        public string WasteDisplay => WasteTypeValue.HasValue ? WasteTypeValue.Value.ToDisplayString() : string.Empty;

        [DisplayName("Compliance year")]
        public string ComplianceYearDisplay => StartDate.Year.ToString();

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public string RecipientAddress { get; set; }

        public bool DisplayEditButton => Status.Equals(NoteStatus.Draft);

        [DisplayName("Date submitted")]
        public string SubmittedDate { get; set; }

        [DisplayName("Date approved")]
        public string ApprovedDate { get; set; }

        public bool HasSubmittedDate => !string.IsNullOrWhiteSpace(SubmittedDate);

        public bool HasApprovedDate => !string.IsNullOrWhiteSpace(ApprovedDate);
    }
}
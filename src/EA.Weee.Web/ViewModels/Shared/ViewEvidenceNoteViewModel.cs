namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.ComponentModel;
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

        public bool HasSubmittedDate => !string.IsNullOrWhiteSpace(SubmittedDate);

        public bool HasApprovedDate => !string.IsNullOrWhiteSpace(ApprovedDate);

        public Guid SchemeId { get; set; }
    }
}
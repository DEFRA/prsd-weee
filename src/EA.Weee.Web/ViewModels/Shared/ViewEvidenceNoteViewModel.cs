namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
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
        public string ComplianceYearDisplay => StartDate.Value.Year.ToString();

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public string RecipientAddress { get; set; }

        public bool DisplayEditButton => Status.Equals(NoteStatus.Draft) || Status.Equals(NoteStatus.Returned);

        public bool HasSubmittedDate => !string.IsNullOrWhiteSpace(SubmittedDate);

        public bool HasApprovedDate => !string.IsNullOrWhiteSpace(ApprovedDate);

        public bool HasRejectedDate => Status.Equals(NoteStatus.Rejected);

        public bool HasBeenReturned => Status.Equals(NoteStatus.Returned);

        public Guid SchemeId { get; set; }

        public string SubmittedBy { get; set; }

        public string TotalReceivedDisplay { get; set; }

        public string AatfApprovalNumber { get; set; }

        public bool DisplayAatfName { get; set; }

        public bool DisplayH2Title { get; set; }

        public int SelectedComplianceYear { get; set; }
    }
}